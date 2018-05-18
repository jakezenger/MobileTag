using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using MobileTag.Models;
using Android.Graphics;

namespace MobileTag.SharedCode
{
    public class MapOverlay
    {
        private PolygonOptions cellPolygonOptions;
        private CircleOptions mineCircleOptions;
        private PolygonOptions antiMineTriangleOptions;
        private Polygon CellPolygon;
        private Circle MineCircle;
        private Polygon AntiMinePolygon;
        public bool CellIsOnMap { get { lock (locker) { return CellPolygon != null; } } }
        public bool MineIsOnMap { get { lock (locker) { return MineCircle != null; } } }
        public bool AntiMineIsOnMap { get { lock (locker) { return AntiMinePolygon != null; } } }
        public int CellID;
        private IMapOverlayClickHandler MapOverlayClickHandler; // Strategy pattern

        private object locker = new object();

        public MapOverlay(Cell cell)
        {
            CellID = cell.ID;

            // Set cell overlay options
            cellPolygonOptions = new PolygonOptions();
            cellPolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude)); //first rectangle point
            cellPolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            cellPolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            cellPolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude)); //automatically connects last two points
            cellPolygonOptions.InvokeZIndex(100);

            // Set mine overlay options
            LatLng circleCenter = new LatLng((double)cell.Latitude + ((double)GameModel.FrontierInterval / 2), (double)cell.Longitude + ((double)GameModel.FrontierInterval / 2));
            mineCircleOptions = new CircleOptions();
            mineCircleOptions.InvokeCenter(circleCenter);
            mineCircleOptions.InvokeRadius(3);
            mineCircleOptions.InvokeStrokeWidth(6);
            mineCircleOptions.InvokeStrokeColor(Color.White);
            mineCircleOptions.InvokeZIndex(200);

            // Set antimine overlay options
            antiMineTriangleOptions = new PolygonOptions();
            antiMineTriangleOptions.Add(new LatLng(((double)cell.Latitude + ((double)GameModel.FrontierInterval) * .75), ((double)cell.Longitude + (double)GameModel.FrontierInterval / 2)));
            antiMineTriangleOptions.Add(new LatLng(((double)cell.Latitude + ((double)GameModel.FrontierInterval) * .25), ((double)cell.Longitude + (double)GameModel.FrontierInterval * .25)));
            antiMineTriangleOptions.Add(new LatLng(((double)cell.Latitude + ((double)GameModel.FrontierInterval) * .25), ((double)cell.Longitude + (double)GameModel.FrontierInterval * .75)));
            antiMineTriangleOptions.InvokeStrokeWidth(6);
            antiMineTriangleOptions.InvokeStrokeColor(Color.White);
            antiMineTriangleOptions.InvokeZIndex(200);

            UpdateColor(cell.HoldStrength, cell.TeamID);
            cellPolygonOptions.InvokeStrokeWidth(0);

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
            try
            {
                if (CellPolygon != null)
                {
                    CellPolygon.FillColor = color;
                    cellPolygonOptions.InvokeFillColor(color);
                }
                else
                    cellPolygonOptions.InvokeFillColor(color);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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

        public void RemoveAntiMine()
        {
            if (AntiMinePolygon != null)
            {
                AntiMinePolygon.Visible = false;
                AntiMinePolygon.Remove();
                AntiMinePolygon = null;
            }
        }

        public void RemoveMine()
        {
            if (MineCircle != null)
            {
                MineCircle.Visible = false;
                MineCircle.Remove();
                MineCircle = null;
            }
        }

        public void Draw(GoogleMap map)
        {
            lock (locker)
            {
                if (!MineIsOnMap && GameModel.Player.Mines.ContainsKey(CellID))
                {
                    MineCircle = map.AddCircle(mineCircleOptions);
                }
                else if (MineIsOnMap && !GameModel.Player.Mines.ContainsKey(CellID))
                {
                    RemoveMine();
                }

                if (!AntiMineIsOnMap && GameModel.Player.AntiMines.ContainsKey(CellID))
                {
                    AntiMinePolygon = map.AddPolygon(antiMineTriangleOptions);
                }
                else if (AntiMineIsOnMap && !GameModel.Player.AntiMines.ContainsKey(CellID))
                {
                    RemoveAntiMine();
                }

                if (!CellIsOnMap)
                {
                    CellPolygon = map.AddPolygon(cellPolygonOptions);
                }
            }
        }

        public void Click(MapActivity mapActivity)
        {
            MapOverlayClickHandler.HandleClickEvent(mapActivity, GameModel.CellsInView[CellID]);
        }
    }
}