using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileTag.SharedCode;

namespace MobileTag.Models
{
    public class AntiMine : Mine
    {
        private Cell cell;
        public int DrainStrength = 100;
        public bool Viable = true;

        public AntiMine(int cellID, int playerID) : base(cellID, playerID)
        {
            if (GameModel.CellsInView.ContainsKey(cellID))
            {
                cell = GameModel.CellsInView[cellID];
            }
        }

        protected override async void Miner_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (cell == null)
            {
                cell = await Database.GetCell(CellID);
            }

            bool stillViable = await cell.Drain(DrainStrength);

            if (stillViable == false)
            {
                Miner.Stop();
                Viable = false;
            }

        }
    }
}