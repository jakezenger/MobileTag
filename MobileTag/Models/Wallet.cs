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
    public class Wallet
    {
        public int Confinium { get; set; }

        public async Task AddConfinium(int amountToAdd)
        {
            int newConfiniumAmount = Confinium + amountToAdd;
            bool successfulDeposit = await Database.UpdatePlayerWallet(GameModel.Player.ID, newConfiniumAmount);

            if (successfulDeposit == true)
            {
                //If database successful, update client player account
                Confinium = newConfiniumAmount;
            }
        }

        public async Task SubtractConfinium(int amountToSubtract)
        {
            if((Confinium - amountToSubtract) > 0)
            {
                int newConfiniumAmount = Confinium - amountToSubtract;
                bool successfulDeposit = await Database.UpdatePlayerWallet(GameModel.Player.ID, newConfiniumAmount);

                if (successfulDeposit == true)
                {
                    //If database successful, update client player account
                    Confinium = newConfiniumAmount;
                }
            }
            else
            {
                // TODO: notify the user that they can't spend the amount they want to
            }
        }
    }
}