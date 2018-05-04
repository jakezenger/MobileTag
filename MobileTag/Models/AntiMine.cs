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
    public class AntiMine
    {
        public int CellID { get; }
        public int PlayerID { get; }
        private Timer Destroyer;
        public int DrainStrength = 100;
        public bool Viable = true;
        public Activity MapActivity { get; set; }

        public AntiMine(int cellID, int playerID)
        {
            CellID = cellID;
            PlayerID = playerID;

            Destroyer = new Timer(5000);
            Destroyer.Elapsed += Destroyer_Elapsed;
        }

        public void Start()
        {
            Destroyer.Start();
        }

        private async void Destroyer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Cell cell = new Cell(CellID);

            if (!GameModel.CellsInView.ContainsKey(CellID))
            {
                cell = await Database.GetCell(CellID);
            }
            else
            {
                MapActivity.RunOnUiThread(() =>
                {
                    cell = GameModel.CellsInView[CellID];
                });

            }

            MapActivity.RunOnUiThread(async () =>
            { 
                bool stillViable = await cell.Drain(DrainStrength);
            
                if (stillViable == false)
                {
                    Destroyer.Stop();
                    Viable = false;
                }
            });
        }
    }
}