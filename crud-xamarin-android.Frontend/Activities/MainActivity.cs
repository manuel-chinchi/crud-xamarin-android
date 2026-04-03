using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;

namespace crud_xamarin_android.Frontend.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button _btnArticles;
        private Button _btnCategories;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            _btnArticles = FindViewById<Button>(Resource.Id.btnArticles_Main);
            _btnArticles.Click += BtnArticles_Click;
            _btnCategories = FindViewById<Button>(Resource.Id.btnCategories_Main);
            _btnCategories.Click += BtnCategories_Click;
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void BtnCategories_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(CategoriesActivity));
            StartActivity(intent);
        }

        private void BtnArticles_Click(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(ArticlesActivity));
            StartActivity(intent);
        }
    }
}