using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Text;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Helpers
{
    public static class ImageHelper
    {
        public static byte[] GetImageAsByteArray(string path)
        {
            byte[] imageBytes = null;
            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        imageBytes = memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return imageBytes;
        }

        public static Java.IO.File CreateImageFile(Activity context)
        {
            string timeStamp = new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date());
            string imageFileName = "JPEG_" + timeStamp + "_";
            Java.IO.File storageDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);

            Java.IO.File image = Java.IO.File.CreateTempFile(
                imageFileName,
                ".jpg",
                storageDir
            );

            return image;
        }

        public static Java.IO.File CreateImageFileFromUri2(Activity context, Android.Net.Uri uri)
        {
            try
            {
                string fileName = "JPEG_" + new SimpleDateFormat("yyyyMMdd_HHmmss").Format(new Date()) + ".jpg";
                Java.IO.File storageDir = context.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures);

                Java.IO.File imageFile = new Java.IO.File(storageDir, fileName);

                using (var inputStream = context.ContentResolver.OpenInputStream(uri))
                {
                    if (inputStream != null)
                    {
                        using (var outputStream = new System.IO.FileStream(imageFile.AbsolutePath, System.IO.FileMode.Create))
                        {
                            inputStream.CopyTo(outputStream);
                        }
                    }
                }

                return imageFile;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Bitmap GetResizedBitmap(Android.Net.Uri imageUri, Activity context)
        {
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            Stream inputStream = context.ContentResolver.OpenInputStream(imageUri);
            BitmapFactory.DecodeStream(inputStream, null, options);
            inputStream.Close();

            int maxHeight = 1024;
            int maxWidth = 1024;

            int scaleFactor = Math.Min(options.OutWidth / maxWidth, options.OutHeight / maxHeight);

            options.InJustDecodeBounds = false;
            options.InSampleSize = scaleFactor;

            inputStream = context.ContentResolver.OpenInputStream(imageUri);
            Bitmap resizedBitmap = BitmapFactory.DecodeStream(inputStream, null, options);
            inputStream.Close();

            return resizedBitmap;
        }

        public static Bitmap GetResizedBitmapFromBytes(byte[] imageData, int maxWidth, int maxHeight)
        {
            try
            {
                BitmapFactory.Options options = new BitmapFactory.Options
                {
                    InJustDecodeBounds = true
                };

                BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

                int imageWidth = options.OutWidth;
                int imageHeight = options.OutHeight;
                int scaleFactor = Math.Min(imageWidth / maxWidth, imageHeight / maxHeight);

                options.InJustDecodeBounds = false;
                options.InSampleSize = scaleFactor;

                return BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetPath(Android.Net.Uri uri, Activity context)
        {
            string path = null;
            String[] projection = { MediaStore.MediaColumns.Data };
            ContentResolver cr =  context.ApplicationContext.ContentResolver;
            var metaCursor = cr.Query(uri, projection, null, null, null);
            if (metaCursor != null)
            {
                try
                {
                    if (metaCursor.MoveToFirst())
                    {
                        path = metaCursor.GetString(0);
                    }
                }
                finally
                {
                    metaCursor.Close();
                }

            }
            return path;
        }
    }
}