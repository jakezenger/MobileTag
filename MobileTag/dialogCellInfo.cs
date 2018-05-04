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

namespace MobileTag
{
    public class dialogCellInfo : DialogFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.dialogCell, container, false);
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            //Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Argb(255, 0, 0, 0)));

            return view;
        }
    }
}