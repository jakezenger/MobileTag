using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileTag.Models;

namespace MobileTag.SharedCode
{
    class FriendlyOverlayClickHandler : IMapOverlayClickHandler
    {
        public void HandleClickEvent(MapActivity mapActivity, Cell cell)
        {
            int playerCellID = 0;

            if (mapActivity.Map.MyLocationEnabled && mapActivity.Map.MyLocation != null)
            {
                playerCellID = Cell.FindID((decimal)mapActivity.Map.MyLocation.Latitude, (decimal)mapActivity.Map.MyLocation.Longitude);
            }

            mapActivity.DisplayStatus(String.Format("Friendly cell! Cell ID: {0}", cell.ID), 3000);
            //mapActivity.DisplayCellInfo(cell.ID);

            if (cell.ID == playerCellID)
                mapActivity.PlantMinePrompt();
        }
    }
}