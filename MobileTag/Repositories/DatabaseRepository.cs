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

namespace MobileTag
{
    class DatabaseRepository : IDatabaseRepository
    {
        public void AddCell(int cellID, decimal lat, decimal lng)
        {
            Database.AddCell(cellID, lat, lng);
        }

        public int AddUser(string username, string password, int teamID)
        {
            return Database.AddUser(username, password, teamID);
        }

        public Cell GetCell(int cellID)
        {
            return Database.GetCell(cellID);
        }

        public int GetCellTeam(int cellID)
        {
            return Database.GetCellTeam(cellID);
        }

        public Player GetPlayer(string username)
        {
            return Database.GetPlayer(username);
        }

        public void UpdateCell(Cell cell, int teamID)
        {
            Database.UpdateCell(cell, teamID);
        }

        public void UpdatePlayerInfo(Player player, string newUsername, string newPassword)
        {
            Database.UpdatePlayerInfo(player, newUsername, newPassword);
        }

        public int ValidateLoginCredentials(string username, string password)
        {
            return Database.ValidateLoginCredentials(username, password);
        }
    }
}