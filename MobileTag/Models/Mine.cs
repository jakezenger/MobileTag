using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileTag.Models
{
    public class Mine
    {
        public int CellID { get; }
        public int PlayerID { get; }
        public int Bucket { get; set; }
        private Timer Miner { get; set; }

        public Mine(int cellID, int playerID, int bucket = 0)
        {
            CellID = cellID;
            PlayerID = playerID;
            Bucket = bucket;

            Miner = new Timer(10000);
            Miner.Elapsed += Miner_Elapsed;
            StartMining();
        }

        public void StartMining()
        {
            Miner.Start();
        }

        private async void Miner_Elapsed(object sender, ElapsedEventArgs e)
        {
            Bucket = await Database.OperateMine(PlayerID, CellID);
        }

        public async Task<int> Yield()
        {
            int bucketTemp = Bucket;
            Bucket = 0;

            await Database.EmptyMineBucket(CellID, PlayerID);

            return bucketTemp;
        }
    }
}