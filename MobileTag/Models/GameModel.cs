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
        public static decimal FrontierInterval => frontierInterval;

      

        // Calculated constants
        public const decimal GridHeight = ((frontierUpperRightLat - frontierLowerLeftLat) / frontierInterval);
        public const decimal GridWidth = ((frontierUpperRightLong - frontierLowerLeftLong) / frontierInterval);

        // Defines how many cells around the target cell to load when using LoadProximalCells
        private const int viewRadius = 10;
        public static int MapStyle = Resource.Raw.style_json;

        // GAMEPLAY CONSTANTS
        public const int maxHoldStrength = 1000;

        public static ConcurrentDictionary<int, Cell> CellsInView = new ConcurrentDictionary<int, Cell>();
        public static Player Player { get; set; }
        private const int DEFAULT_TAG_AMOUNT = 100;
        public const int MINE_BASE_PRICE = 500;
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

        public async static Task LoadProximalCells(LatLng targetLatLng)
        {
            int targetCellID = Cell.FindID((decimal)targetLatLng.Latitude, (decimal)targetLatLng.Longitude);
            var ProximalCells = await Database.GetProxyCells(viewRadius, frontierInterval, (decimal)targetLatLng.Latitude, (decimal)targetLatLng.Longitude);
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
                }

                for (int row = -viewRadius; row <= viewRadius; row++)
                {
                    for (int col = -viewRadius; col <= viewRadius; col++)
                    {
                        int cellID = (int)(targetCellID + (row * GridWidth) + col);

                        if (!CellsInView.ContainsKey(cellID))
                        {
                            NewSubscriptions.Add(cellID);
                        }
                    }
                }
            });

            if (NewSubscriptions.Count > 0)
            {
                await CellHub.SubscribeToUpdates(NewSubscriptions);
            }
        }

        internal async static Task AddCurrency()
        {
            //TODO: Send currency to database
            int moneyToDeposit = Player.Wallet.Confinium + DEFAULT_TAG_AMOUNT;
            bool successfulDeposit = await Database.UpdatePlayerWallet(Player.ID, moneyToDeposit);
            if (successfulDeposit == true)
            {
                //If database successful, update client player account
                 Player.Wallet.AddConfinium(DEFAULT_TAG_AMOUNT);
            }
            
        }
    }
}