using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Util;
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
    public class CameraHelper
    {
        public const int REQUEST_CAMERA_PERMISSION = 100;
        public const int REQUEST_OPEN_CAMERA = 1;

        public static bool CheckCameraPermission(int requestCode, [GeneratedEnum] Permission[] grantResults)
        {
            bool permission = false;
            if (requestCode == REQUEST_CAMERA_PERMISSION)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    permission = true;
            }
            return permission;
        }

        public static bool HasCameraPermission(Activity context)
        {
            bool permission = context.CheckSelfPermission(Android.Manifest.Permission.Camera) == (int)Android.Content.PM.Permission.Granted;
            return permission;
        }

        public static void RequestCameraPermission(Activity context)
        {
            context.RequestPermissions(new string[] { Android.Manifest.Permission.Camera }, REQUEST_CAMERA_PERMISSION);
        }

        public static bool CheckResultCamera(int requestCode, Result resultCode)
        {
            return requestCode == REQUEST_OPEN_CAMERA && resultCode == Result.Ok;
        }
    }
}