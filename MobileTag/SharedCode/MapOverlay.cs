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
        public PolygonOptions overlay { get; }

        public MapOverlay(Cell cell)
        {
            PolygonOptions squareOverlay = new PolygonOptions();
            squareOverlay.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude)); //first rectangle point
            squareOverlay.Add(new LatLng((double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            squareOverlay.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval));
            squareOverlay.Add(new LatLng((double)cell.Latitude + (double)GameModel.FrontierInterval, (double)cell.Longitude)); //automatically connects last two points

            Color color = ColorCode.TeamColor(cell.TeamID);
            squareOverlay.InvokeFillColor(color); //Transparent (alpha) int [0-255] 255 being opaque
            squareOverlay.InvokeStrokeWidth(0);

            overlay = squareOverlay;
        }

        public void UpdateColor()
        {
            Color color = ColorCode.TeamColor(GameModel.Player.Team.ID);
            overlay.InvokeFillColor(color);
        }

        public void SetColor(Color color)
        {
            overlay.InvokeFillColor(color);
        }
    }
}