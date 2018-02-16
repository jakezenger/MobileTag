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

namespace MobileTag.Repository
{
    interface IDatabaseRepository
    {
        int AddUser(String Username, String Password, int id);
        int ValidateLoginCredentials();
        int GetCellTeam();
        Player GetPlayer();
        Cell GetCell();
        void DeletePlayer();
        void DeletecCell();
        void UpdatePlayerInfo();
    }
}