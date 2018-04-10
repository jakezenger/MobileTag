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

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MobileTag.Models;
using Android.Graphics;

namespace MobileTag.SharedCode
{
    public class MapOverlay
    {
        private PolygonOptions PolygonOptions;
        private Polygon Polygon;
        public bool IsOnMap { get { lock (locker) { return Polygon != null; } } }
        public int CellID;
        private IMapOverlayClickHandler MapOverlayClickHandler; // Strategy pattern

        private object locker = new object();

        public MapOverlay(Cell cell)
        {
            CellID = cell.ID;

            PolygonOptions = new PolygonOptions();
            PolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude)); //first rectangle point
            PolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            PolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            PolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude)); //automatically connects last two points

            UpdateColor(cell.HoldStrength, cell.TeamID);
            PolygonOptions.InvokeStrokeWidth(0);

            if (cell.TeamID == GameModel.Player.Team.ID)
            {
                MapOverlayClickHandler = new FriendlyOverlayClickHandler();
            }
            else
            {
                MapOverlayClickHandler = new EnemyOverlayClickHandler();
            }
        }

        private void SetColor(Color color)
        {
            if (Polygon != null)
            {
                Polygon.FillColor = color;
                PolygonOptions.InvokeFillColor(color);
            }
            else
                PolygonOptions.InvokeFillColor(color);
        }

        public void UpdateColor(int holdStrength, int teamID)
        {
            if (teamID != GameModel.Player.Team.ID && MapOverlayClickHandler is FriendlyOverlayClickHandler)
            {
                MapOverlayClickHandler = new EnemyOverlayClickHandler();
            }
            else if (teamID == GameModel.Player.Team.ID && MapOverlayClickHandler is EnemyOverlayClickHandler)
            {
                MapOverlayClickHandler = new FriendlyOverlayClickHandler();
            }

            byte alpha = Convert.ToByte((int)(((float)holdStrength / GameModel.maxHoldStrength) * 255));

            if (alpha > 255)
                alpha = 255;

            Color overlayColor = ColorCode.TeamColor(teamID);
            overlayColor.A = alpha;

            SetColor(overlayColor);
        }

        public void Draw(GoogleMap map)
        {
            lock (locker)
            {
                if (!IsOnMap)
                    Polygon = map.AddPolygon(PolygonOptions);
            }
        }

        public void Click(MapActivity mapActivity)
        {
            MapOverlayClickHandler.HandleClickEvent(mapActivity, GameModel.CellsInView[CellID]);
        }
    }
}