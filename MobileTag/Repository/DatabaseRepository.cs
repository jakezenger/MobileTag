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
    class DatabaseRepository : IDatabaseRepository
    {
        public int AddUser(String Username, String Password, int id)
        {
           return Database.AddUser(Username, Password, id);
        }

        public void DeletecCell()
        {
            throw new NotImplementedException();
        }

        public void DeletePlayer()
        {
            throw new NotImplementedException();
        }

        public Cell GetCell()
        {
            throw new NotImplementedException();
        }

        public int GetCellTeam()
        {
            throw new NotImplementedException();
        }

        public Player GetPlayer()
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayerInfo()
        {
            throw new NotImplementedException();
        }

        public int ValidateLoginCredentials()
        {
            throw new NotImplementedException();
        }
    }
}