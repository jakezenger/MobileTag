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
            squareOverlay.Add(new LatLng(90.0 - (double)cell.Latitude, (double)cell.Longitude - 180.0)); //first rectangle point
            squareOverlay.Add(new LatLng(90.0 - (double)cell.Latitude, (double)cell.Longitude + (double)GameModel.FrontierInterval - 180.0));
            squareOverlay.Add(new LatLng(90.0 -(double)cell.Latitude - (double)GameModel.FrontierInterval, (double)cell.Longitude + (double)GameModel.FrontierInterval - 180.0));
            squareOverlay.Add(new LatLng(90.0 -(double)cell.Latitude - (double)GameModel.FrontierInterval, (double)cell.Longitude - 180.0)); //automatically connects last two points

            Color color = ColorCode.SetTeamColor(cell.TeamID);
            squareOverlay.InvokeFillColor(color); //Transparent (alpha) int [0-255] 255 being opaque
            squareOverlay.InvokeStrokeWidth(0);

            overlay = squareOverlay;
        }

        public void UpdateColor()
        {
            Color color = ColorCode.SetTeamColor(GameModel.Player.Team.ID);
            overlay.InvokeFillColor(color);
        }
    }
}