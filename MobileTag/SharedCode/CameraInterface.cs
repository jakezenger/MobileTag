using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MobileTag.SharedCode;

namespace MobileTag.SharedCode
{

    public class CameraInterface : ICameraInterface
    {
        public CameraInterface()
        {
        }

        public void BringUpCamera()
        {
            var intent = new Intent(MediaStore.ActionImageCapture);
            ((Activity)Forms.Context).StartActivityForResult(intent, 1);
        }

        public void BringUpPhotoGallery()
        {
            var imageIntent = new Intent();
            imageIntent.SetType("image/*");
            imageIntent.SetAction(Intent.ActionGetContent);

            ((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 1);
        }


        
    }
}