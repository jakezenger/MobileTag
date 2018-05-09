using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileTag.Models;

namespace MobileTag
{
    public class dialogCellInfo : DialogFragment
    {
        private View view;
        private TextView cellText;
        private TextView teamText;
        private Cell cellClicked;
        private ImageButton mineBtn;
        private ImageButton yieldBtn;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(Resource.Layout.dialogCell, container, false);
            cellText = (TextView)view.FindViewById(Resource.Id.cellTextID);
            teamText = (TextView)view.FindViewById(Resource.Id.teamText);
            mineBtn = (ImageButton)view.FindViewById(Resource.Id.minePlaceBtn);
            yieldBtn = (ImageButton)view.FindViewById(Resource.Id.yieldBtn);
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Argb(255, 0, 0, 0)));
            mineBtn.Click += MineBtn_Click;
            yieldBtn.Click += YieldBtn_Click;

            SetCellText(cellClicked);
            SetTeamColor(cellClicked);
            return view;
        }

        private void YieldBtn_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Context, "Not yet implemented!", ToastLength.Long).Show();
        }

        private async void MineBtn_Click(object sender, EventArgs e)
        {
            if (GameModel.Player.Wallet.Confinium >= GameModel.MINE_BASE_PRICE)
            {
                await GameModel.Player.CreateMine(cellClicked.ID);
                Toast.MakeText(Context, "Mine Placed!", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Context, "You don't have enough confinium in your wallet to purchase a mine. ", ToastLength.Long).Show();
            }
        }

        public dialogCellInfo(Cell cell)
        {
            cellClicked = cell;
        }

        private void SetCellText(Cell cell)
        {
            cellText.Text = cell.ID.ToString();
            cellText.SetTextColor(ColorCode.TeamColor(cell.TeamID));
        }

        private void SetTeamColor(Cell cell)
        {
            teamText.Text = ColorCode.TeamName(cell.TeamID);
            teamText.SetTextColor(ColorCode.TeamColor(cell.TeamID));
        }
    }
}