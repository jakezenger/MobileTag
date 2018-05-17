using System;
using Android.Content;
using Android.Views;
using Android.Text.Style;
using Android.Graphics;
using Android.Text;

namespace MobileTag
{
    public class ActivityLink : ClickableSpan
    {
        private Context context;
        private Type destType;

        public ActivityLink(Context context, Type destinationType) : base()
        {
            this.context = context;
            this.destType = destinationType;
        }
        public override void OnClick(View widget)
        {
            Intent intent = new Intent(context, destType);
            context.StartActivity(intent);
        }
        public override void UpdateDrawState(TextPaint ds)
        {
            base.UpdateDrawState(ds);
            ds.Color = Color.Blue;
            ds.UnderlineText = false;
        }
    }
}