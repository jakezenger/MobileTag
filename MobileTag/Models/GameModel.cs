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
using Microsoft.AspNet.SignalR.Client;

namespace MobileTag.Models
{
    public class GameModel : Application
    {
        // IMPORTANT: corresponds to Frontier's specifications.
        // Do not change this unless you first recreate the Frontier's cell database using your new specifications.
        private const decimal frontierInterval = .00009375m;
        private const decimal frontierUpperLeftLat = 44.677684m;
        private const decimal frontierUpperLeftLong = 57.232579m;
        private const decimal frontierLowerRightLat = 44.679184m;
        private const decimal frontierLowerRightLong = 57.234079m;

        public static decimal FrontierInterval => frontierInterval;

        public static ConcurrentDictionary<int, Cell> CellsInView = new ConcurrentDictionary<int, Cell>();
        public static Player Player { get; set; }

        // SignalR
        public static HubConnection CellHubConnection = new HubConnection("https://mobiletag.azurewebsites.net/");
        public static IHubProxy CellHubProxy;


        public static int GetCellID(decimal lat, decimal lng)
        {
            decimal nearestLatInterval, nearestLongInterval;

            nearestLatInterval = (Math.Floor((lat - frontierUpperLeftLat) / frontierInterval) * frontierInterval);
            nearestLongInterval = (Math.Floor((lng - frontierUpperLeftLong) / frontierInterval) * frontierInterval);

            int id = Convert.ToInt32((nearestLongInterval / frontierInterval) + (nearestLatInterval / frontierInterval) 
                * ((frontierLowerRightLong - frontierUpperLeftLong) / frontierInterval));

            return id;
        }

        async static public void SubscribeToUpdates(HashSet<int> cellIDs)
        {
            try
            {
                await GameModel.CellHubProxy.Invoke("SubscribeToCellUpdates", cellIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        async static public void SubscribeToUpdates(ConcurrentDictionary<int, Cell> cellsInView)
        {
            HashSet<int> cellIDs = new HashSet<int>();

            foreach(Cell cell in cellsInView.Values)
            {
                cellIDs.Add(cell.ID);
            }

            try
            {
                await GameModel.CellHubProxy.Invoke("SubscribeToCellUpdates",cellIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        async static public void SubscribeToUpdates()
        {
            HashSet<int> cellIDs = new HashSet<int>();

            foreach (Cell cell in CellsInView.Values)
            {
                cellIDs.Add(cell.ID);
            }

            try
            {
                await GameModel.CellHubProxy.Invoke("SubscribeToCellUpdates", cellIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //async static public void UnsubscribeFromUpdates(int cellID)
        //{
        //    try
        //    {
        //        await GameModel.CellHubProxy.Invoke("UnsubscribeFromCellUpdates", cellID);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}

        //TODO: Make this work with Android/Google API's
        public Color GetTeamColor(int teamID)
        {
            Color color;
            switch(teamID)
            {
                case 1: color = new Color(); break; //RED
                case 2: color = new Color(); break; //GREEN
                case 3: color = new Color(); break; //BLUE
                case 4: color = new Color(); break; //PURPLE
                case 5: color = new Color(); break; //PINK

                default: color = new Color(); break;    //TRANSPARENT         
            }
            return color;
        }
    }
}