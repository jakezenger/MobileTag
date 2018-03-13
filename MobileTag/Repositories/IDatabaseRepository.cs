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
    interface IDatabaseRepository
    {
       int AddUser(string username, string password, int teamID);
       int ValidateLoginCredentials(string username, string password);
        int GetCellTeam(int cellID);
        Player GetPlayer(string username);
        Cell GetCell(int cellID);
        void UpdateCell(Cell cell, int teamID);
        void UpdatePlayerInfo(Player player, string newUsername, string newPassword);
        void AddCell(int cellID, decimal lat, decimal lng);

    }
}