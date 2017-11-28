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
using Android.Graphics;

namespace MobileTag.Models
{
    public class Player
    {
        public int ID { get; }
        public Team Team { get; set; }
        public int CurrentCellID { get; set; }
        public Color teamColor { get; set; }

        public Player(int id, Team team, int currentCellID)
        {
            ID = id;
            Team = team;
            CurrentCellID = currentCellID;
            teamColor = ColorCode.SetTeamColor(Team.ID);
        }

        public Player(int id, Team team, decimal lat, decimal lng)
        {
            ID = id;
            Team = team;
            CurrentCellID = GameModel.GetCellID(lat, lng);
            teamColor = ColorCode.SetTeamColor(team.ID);
        }

        public Player(Player player)
        {
            ID = player.ID;
            Team = player.Team;
            CurrentCellID = player.CurrentCellID;
            teamColor = ColorCode.SetTeamColor(Team.ID);
        }


    }
}