using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        async static public Task SubscribeToCellUpdates(HashSet<int> cellIDs)
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

        async static public Task SubscribeToCellUpdates(ConcurrentDictionary<int, Cell> cellsInView)
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

        async static public Task SubscribeToCellUpdates(int cellID)
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