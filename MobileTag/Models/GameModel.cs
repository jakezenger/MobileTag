using System;

using Android.App;

using System.Drawing;

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

        //public List<Cell> CellsInView { get; set; } -- SHOULD MAYBE GO IN MAP ACTIVITY... DOES THIS NEED TO BE HERE?
        public static Player Player { get; set; }

        public static int GetCellID(decimal lat, decimal lng)
        {
            decimal nearestLatInterval, nearestLongInterval;

            nearestLatInterval = (Math.Floor((lat - frontierUpperLeftLat) / frontierInterval) * frontierInterval);
            nearestLongInterval = (Math.Floor((lng - frontierUpperLeftLong) / frontierInterval) * frontierInterval);

            int id = Convert.ToInt32((nearestLongInterval / frontierInterval) + (nearestLatInterval / frontierInterval) 
                * ((frontierLowerRightLong - frontierUpperLeftLong) / frontierInterval));

            return id;
        }
        
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