using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Helpers
{
    public static class GaleryHelper
    {
        public const int REQUEST_GALLERY_PERMISSION = 101;
        public const int REQUEST_OPEN_GALLERY = 2;
        
        public static bool CheckGaleryPermission(int requestCode, [GeneratedEnum] Permission[] grantResults)
        {
            bool permission=false;
            if (requestCode == REQUEST_GALLERY_PERMISSION)
            {
                if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
                    permission = true;
            }
            return permission;
        }

        public static bool HasGaleryPermission(Activity context)
        {
            bool permission = context.CheckSelfPermission(Android.Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted;
            return permission;
        }

        public static void RequestGaleryPermission(Activity context)
        {
            context.RequestPermissions(new string[] { Android.Manifest.Permission.ReadExternalStorage }, REQUEST_GALLERY_PERMISSION);
        }

        public static bool CheckResultGalery(int requestCode, [GeneratedEnum] Result resultCode)
        {
            return requestCode == REQUEST_OPEN_GALLERY && resultCode == Result.Ok;
        }
    }
}