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
using System.Collections.Concurrent;

namespace MobileTag.Models
{
    public class Player
    {
        public string Username { get; set; }
        public int ID { get; }
        public Team Team { get; set; }
        public int CurrentCellID { get; set; }
        public Wallet Wallet { get; set; }
        public ConcurrentDictionary<int, Mine> Mines { get; }
        public List<AntiMine> AntiMines { get; }

        public Player(int id, string username, Team team, int currentCellID, ConcurrentDictionary<int, Mine> mines, List<AntiMine> aMines, Wallet wallet)
        {
            ID = id;
            Team = team;
            CurrentCellID = currentCellID;
            Username = username;
            Wallet = wallet;
            Mines = mines;
            AntiMines = aMines;
            //TODO: ADD ANTI MINES EVERYWHERE
        }

        public Player(int id, string username, Team team, decimal lat, decimal lng, ConcurrentDictionary<int, Mine> mines, List<AntiMine> aMines, Wallet wallet)
        {
            ID = id;
            Team = team;
            CurrentCellID = Cell.FindID(lat, lng);
            Username = username;
            Wallet = wallet;
            Mines = mines;
            AntiMines = aMines;
        }

        public async Task<Mine> CreateMine(int cellID)
        {
            Mine mine = new Mine(cellID, ID);
            await Database.AddMine(ID, cellID);
            await Wallet.SubtractConfinium(GameModel.MINE_BASE_PRICE);

            Mines.TryAdd(cellID, mine);

            return mine;
        }

        public async Task<AntiMine> CreateAntiMine(int cellID)
        {
            AntiMine aMine = new AntiMine(cellID, ID);
            await Database.AddAntiMine(ID, cellID);
            await Wallet.SubtractConfinium(GameModel.ANTI_MINE_BASE_PRICE);
            await Database.UpdatePlayerWallet(ID, Wallet.Confinium);
            AntiMines.Add(aMine);

            return aMine;
        }

        public void StartAntiMines(Activity mapActivity)
        {
            foreach (AntiMine am in AntiMines)
            {
                am.MapActivity = mapActivity;
                am.Start();
            }
        }
    }
}