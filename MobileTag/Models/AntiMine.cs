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

        public void Stop()
        {
            Destroyer.Stop();
        }

        private async void Destroyer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
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
                    Viable = await cell.Drain(DrainStrength);
                });
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
        }
    }
}