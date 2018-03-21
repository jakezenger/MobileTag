using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AspNet.SignalR.Client;
using MobileTag.Models;

namespace MobileTag.SharedCode
{
    static class CellHub
    {
        // SignalR
        public static HubConnection Connection = new HubConnection("https://mobiletag.azurewebsites.net/");
        public static IHubProxy HubProxy;

        static CellHub()
        {
            HubProxy = Connection.CreateHubProxy("cellHub");
        }

        async static public Task SubscribeToUpdates(HashSet<int> cellIDs)
        {
            try
            {
                await HubProxy.Invoke("SubscribeToCellUpdates", cellIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        async static public Task SubscribeToUpdates(ConcurrentDictionary<int, Cell> cellsInView)
        {
            HashSet<int> cellIDs = new HashSet<int>();

            foreach(Cell cell in cellsInView.Values)
            {
                cellIDs.Add(cell.ID);
            }

            try
            {
                await HubProxy.Invoke("SubscribeToCellUpdates",cellIDs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        async static public Task SubscribeToUpdates(int cellID)
        {
            try
            {
                await HubProxy.Invoke("SubscribeToSingleCellUpdates", cellID);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}