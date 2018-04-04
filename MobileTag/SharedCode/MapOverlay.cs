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
        public PolygonOptions PolygonOptions { get; }
        public Polygon Polygon { get; set; }
        public bool IsOnMap { get { lock (locker) { return Polygon != null; } } }
        public int CellID;

        public readonly object locker = new object();

        public MapOverlay(Cell cell)
        {
            CellID = cell.ID;

            PolygonOptions = new PolygonOptions();
            PolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude)); //first rectangle point
            PolygonOptions.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            PolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            PolygonOptions.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude)); //automatically connects last two points

            Color color = ColorCode.TeamColor(cell.TeamID);
            PolygonOptions.InvokeFillColor(color); //Transparent (alpha) int [0-255] 255 being opaque
            PolygonOptions.InvokeStrokeWidth(0);
        }

        public void SetColor(Color color)
        {
            if (Polygon != null)
                Polygon.FillColor = color;
        }

        public void Draw(GoogleMap map)
        {
            lock (locker)
            {
                if (!IsOnMap)
                    Polygon = map.AddPolygon(PolygonOptions);
            }
        }
    }
}