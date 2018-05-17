using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileTag.SharedCode
{
    public class BitmapWorkerTask : AsyncTask<int, int, Bitmap>
    {
        private Android.Net.Uri uriReference;
        private int data = 0;
        private ContentResolver resolver;

        public BitmapWorkerTask(ContentResolver cr, Android.Net.Uri uri)
        {
            uriReference = uri;
            resolver = cr;
        }

        // Decode image in background.
        protected override Bitmap RunInBackground(params int[] p)
        {
            //This will be the orientation that was passed in from above (task.Execute(orientation);)
            data = p[0];

            Bitmap mBitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(resolver, uriReference);
            Bitmap myBitmap = null;

            if (mBitmap != null)
            {
                //In order to rotate the image we create a Matrix object, rotate if the image is not already in it's correct orientation
                Matrix matrix = new Matrix();
                if (data != 0)
                {
                    matrix.PreRotate(data);
                }

                myBitmap = Bitmap.CreateBitmap(mBitmap, 0, 0, mBitmap.Width, mBitmap.Height, matrix, true);
                return myBitmap;
            }

            return null;
        }

        //Called when the RunInBackground method has finished
        protected override void OnPostExecute(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();

                //Compressing by 50%, feel free to change if file size is not a factor
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 50, stream);
                byte[] bitmapData = stream.ToArray();

                //Send image byte array back to UI
                MessagingCenter.Send<byte[]>(bitmapData, "ImageSelected");

                //clean up bitmaps so the app doesn't crash from using up too much memory
                bitmap.Recycle();
                GC.Collect();
            }
        }
    }
}