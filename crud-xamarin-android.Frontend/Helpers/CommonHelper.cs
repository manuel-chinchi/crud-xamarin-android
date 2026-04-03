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
    public static class CommonHelper
    {
        public static string GetFileProviderAuthorities(Activity context)
        {
            try
            {
                PackageManager packageManager = context.PackageManager;
                ApplicationInfo appInfo = packageManager.GetApplicationInfo(context.PackageName, PackageInfoFlags.MetaData);
                IList<ProviderInfo> providers= packageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.Providers).Providers;

                foreach (var provider in providers)
                {
                    if (provider.Name == "androidx.core.content.FileProvider")
                    {
                        // com.companyname.crud_xamarin.fileprovider
                        return provider.Authority;
                    }
                }
            }
            catch (PackageManager.NameNotFoundException ex)
            {
                throw ex;
            }

            return null;
        }
    }
}