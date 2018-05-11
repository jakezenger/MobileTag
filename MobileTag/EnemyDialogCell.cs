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
using Microcharts;
using Microcharts.Droid;
using MobileTag.Models;
using SkiaSharp;

namespace MobileTag
{
    public class EnemyDialogCell : DialogFragment
    {
        private View view;
        private TextView cellText;
        private TextView teamText;
        private Cell cellClicked;
        private ImageButton antiMineBtn;
        private ImageButton yieldBtn;
        private ChartView donutChart;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            view = inflater.Inflate(Resource.Layout.EnemyDialogCell, container, false);
            cellText = (TextView)view.FindViewById(Resource.Id.cellTextID);
            teamText = (TextView)view.FindViewById(Resource.Id.teamText);
            antiMineBtn = (ImageButton)view.FindViewById(Resource.Id.antiMinePlaceBtn);
            yieldBtn = (ImageButton)view.FindViewById(Resource.Id.yieldBtnEnemy);
            donutChart = (ChartView)view.FindViewById(Resource.Id.chartViewEnemy);
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Argb(255, 0, 0, 0)));
            antiMineBtn.Click += AntiMineBtn_Click;
            yieldBtn.Click += YieldBtn_Click;

            SetCellText(cellClicked);
            SetTeamColor(cellClicked);
            SetChart();
            return view;
        }

        private void YieldBtn_Click(object sender, EventArgs e)
        {
            Toast.MakeText(Context, "Not yet implemented!", ToastLength.Long).Show();
        }

        private async void AntiMineBtn_Click(object sender, EventArgs e)
        {
            if (GameModel.Player.Wallet.Confinium >= GameModel.ANTI_MINE_BASE_PRICE)
            {
                await GameModel.Player.CreateAntiMine(cellClicked.ID);
                Toast.MakeText(Context, "Anti Mine Placed!", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(Context, "You don't have enough confinium in your wallet to purchase an AntiMine. ", ToastLength.Long).Show();
            }
        }

        public EnemyDialogCell(Cell cell)
        {
            cellClicked = cell;
        }

        private void SetCellText(Cell cell)
        {
            cellText.Text = cell.ID.ToString();
            cellText.SetTextColor(ColorCode.BrightTeamColor(cell.TeamID));
        }

        private void SetTeamColor(Cell cell)
        {
            teamText.Text = ColorCode.TeamName(cell.TeamID);
            teamText.SetTextColor(ColorCode.BrightTeamColor(cell.TeamID));
        }

        private void SetChart()
        {
            /*** Foreach loop gets the same cell clicked for this dialog fragment. 
             *** The loop is needed to get holdstrength
             ***/
            foreach (var cell in GameModel.CellsInView)
            {
                if (cell.Value.ID == cellClicked.ID)
                {
                    cellClicked.HoldStrength = cell.Value.HoldStrength;
                    break;
                }
            }

            List<Entry> entries = new List<Entry>
            {
            new Entry(1000-cellClicked.HoldStrength)
            {
                Color=SKColor.Parse("#ffffff"),
                Label ="Remaining",
                ValueLabel = (1000-cellClicked.HoldStrength).ToString()
            },
            new Entry(cellClicked.HoldStrength)
            {
                Color = SKColor.Parse(ColorCode.BrightHexColorCode(cellClicked.TeamID)),
                Label = "Hold Strength",
                ValueLabel = cellClicked.HoldStrength.ToString()
            },
            };

            DonutChart chart = new DonutChart()
            {
                Entries = entries,
                HoleRadius = 0.6f,
                BackgroundColor = SKColor.Parse("#202020")
            };

            donutChart.Chart = chart;
        }
    }
}
