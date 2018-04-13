using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileTag.Models
{
    public class Player
    {
        public string Username { get; set; }
        public int ID { get; }
        public Team Team { get; set; }
        public int CurrentCellID { get; set; }
        public Wallet Wallet { get; set; }
        public List<Mine> Mines { get; }

        public Player(int id, string username, Team team, int currentCellID, List<Mine> mines, Wallet wallet)
        {
            ID = id;
            Team = team;
            CurrentCellID = currentCellID;
            Username = username;
            Wallet = wallet;
            Mines = mines;
        }

        public Player(int id, string username, Team team, decimal lat, decimal lng, List<Mine> mines, Wallet wallet)
        {
            ID = id;
            Team = team;
            CurrentCellID = Cell.FindID(lat, lng);
            Username = username;
            Wallet = wallet;
            Mines = mines;
        }

        public async Task<Mine> CreateMine(int cellID)
        {
            Mine mine = new Mine(cellID, ID);
            await Database.AddMine(GameModel.Player.ID, cellID);
            Mines.Add(mine);

            return mine;
        }
    }
}