using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Drawing;
using Android.Gms.Maps.Model;
using MobileTag.SharedCode;
using System.Threading.Tasks;

namespace MobileTag.Models
{
    public class GameModel : Application
    {
        // IMPORTANT: corresponds to Frontier's specifications.
        // Do not change this unless you first recreate the Frontier's cell database using your new specifications.
        public const decimal frontierInterval = .0001m;
        public const decimal frontierLowerLeftLat = 43.0m;
        public const decimal frontierLowerLeftLong = -124.5m;
        public const decimal frontierUpperRightLat = 47.0m;
        public const decimal frontierUpperRightLong = -116.5m;
        private const int viewRadius = 10;

        public static decimal FrontierInterval => frontierInterval;

        public static ConcurrentDictionary<int, Cell> CellsInView = new ConcurrentDictionary<int, Cell>();
        public static Player Player { get; set; }

        // Calculated constants
        public const decimal GridHeight = ((frontierUpperRightLat - frontierLowerLeftLat) / frontierInterval);
        public const decimal GridWidth = ((frontierUpperRightLong - frontierLowerLeftLong) / frontierInterval);

        public static void Logout()
        {
            string path = Application.Context.FilesDir.Path;
            var filePath = System.IO.Path.Combine(path, "username.txt");
            var filePath2 = System.IO.Path.Combine(path, "password.txt");

            try
            {
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(filePath2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        public static int GetCellID(decimal lat, decimal lng)
        {
            decimal nearestLatInterval, nearestLongInterval;

            nearestLatInterval = (Math.Floor((lat - frontierLowerLeftLat) / frontierInterval) * frontierInterval);
            nearestLongInterval = (Math.Floor((lng - frontierLowerLeftLong) / frontierInterval) * frontierInterval);

            int id = Convert.ToInt32((nearestLongInterval / frontierInterval) + (nearestLatInterval / frontierInterval)
                * GridWidth);

            return id;
        }

        public static LatLng GetLatLng(int cellID)
        {
            decimal lat = frontierLowerLeftLat + (cellID / GridWidth * frontierInterval);
            decimal lng = frontierLowerLeftLong + (cellID % GridWidth * frontierInterval);

            LatLng latLng = new LatLng((double)lat, (double)lng);

            return latLng;
        }

        public async static Task<ConcurrentDictionary<int, MapOverlay>> LoadProximalCells(LatLng targetLatLng)
        {
            var Overlays = new ConcurrentDictionary<int, MapOverlay>();
            var ProximalCells = await RetrieveProximalCells(targetLatLng);
            var NewSubscriptions = new HashSet<int>();

            await Task.Run(() =>
            {
                foreach (Cell cell in ProximalCells.Values)
                {
                    if (!CellsInView.ContainsKey(cell.ID))
                    {
                        CellsInView.TryAdd(cell.ID, cell);
                        NewSubscriptions.Add(cell.ID);
                    }

                    if (cell.TeamID > 0)
                    {
                        Overlays.TryAdd(cell.ID, new MapOverlay(cell));
                    }
                }
            });

            if (NewSubscriptions.Count > 0)
            {
                await CellHub.SubscribeToUpdates(NewSubscriptions);
            }

            return Overlays;
        }

        private async static Task<ConcurrentDictionary<int, Cell>> RetrieveProximalCells(LatLng targetLatLng)
        {
            int playerCellID = GetCellID((decimal)targetLatLng.Latitude, (decimal)targetLatLng.Longitude);
            ConcurrentDictionary<int, Cell> frontierDict = await Database.GetProxyCells(viewRadius, frontierInterval, (decimal)targetLatLng.Latitude, (decimal)targetLatLng.Longitude);

            await Task.Run(() =>
            {
                for (int row = -viewRadius; row <= viewRadius; row++)
                {
                    for (int col = -viewRadius; col <= viewRadius; col++)
                    {
                        int cellID = (int)(playerCellID + (row * GridWidth) + col);

                        if (!CellsInView.ContainsKey(cellID))
                        {
                            decimal cellLat = Math.Floor((decimal)targetLatLng.Latitude / frontierInterval) * frontierInterval + (row * frontierInterval);
                            decimal cellLng = Math.Floor((decimal)targetLatLng.Longitude / frontierInterval) * frontierInterval + (col * frontierInterval);
                            Cell cell = new Cell(cellLat, cellLng);

                            frontierDict.TryAdd(cellID, cell);
                        }
                    }

                }
            });

            return frontierDict;
        }

        public static Cell GenerateCell(decimal lat, decimal lng)
        {
            decimal cellLat = Math.Floor(lat / frontierInterval) * frontierInterval;
            decimal cellLng = Math.Floor(lng / frontierInterval) * frontierInterval;
            return new Cell(cellLat, cellLng);
        }
    }
}