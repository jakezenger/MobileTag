using System;
using MobileTag.Models;

namespace MobileTag.SharedCode
{
    class EnemyOverlayClickHandler : IMapOverlayClickHandler
    {
        public void HandleClickEvent(MapActivity mapActivity, Cell cell)
        {
            int playerCellID = 0;

            if (mapActivity.Map.MyLocation != null)
            {
                playerCellID = Cell.FindID((decimal)mapActivity.Map.MyLocation.Latitude, (decimal)mapActivity.Map.MyLocation.Longitude);
            }

            mapActivity.DisplayStatus(String.Format("Enemy cell! Cell ID: {0}", cell.ID), 3000);

            if (cell.ID == playerCellID)
            {
                mapActivity.PlantAntiMinePrompt();
            }
            else
            {
                //mapActivity.DefaultCellDialog();
            }

        }
    }
}