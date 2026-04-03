using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.Content;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.ViewModels;
using crud_xamarin_android.Frontend.Activities.Contracts;
using crud_xamarin_android.Frontend.Helpers;
using crud_xamarin_android.Frontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace crud_xamarin_android.Frontend.Activities
{
    [Activity(Label = "")]
    public class CreateArticleActivity : AppCompatActivity, IBaseActivity
    {
        private Button _btnAccept;
        private Button _btnCancel;
        private EditText _etName;
        private EditText _etDetails;
        private ImageView _imgImage;
        private TextView _txtDeleteImage;
        private Spinner _spnCategory;

        private static List<Category> s_categories;
        private static Java.IO.File s_image;
        private readonly CreateArticleViewModel _viewModel;
        private readonly ObservableImageManager _imageManager;

        public CreateArticleActivity()
        {
            _viewModel = new CreateArticleViewModel();
            _imageManager = new ObservableImageManager();
            s_categories = _viewModel.Categories.ToList();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_create_article);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            InitializeControls();

            BindControls();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        protected override void OnActivityResult(
            int requestCode,
            [GeneratedEnum] Result resultCode,
            Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (CameraHelper.CheckResultCamera(requestCode, resultCode))
            {
                _imgImage.SetImageURI(Android.Net.Uri.Parse(s_image.AbsolutePath));
                _txtDeleteImage.Visibility = ViewStates.Visible;

                _imageManager.UpdateFromFile(s_image);
            }

            if (GaleryHelper.CheckResultGalery(requestCode, resultCode))
            {
                if (data != null)
                {
                    var imageUri = data.Data;
                    var bitmap = ImageHelper.GetResizedBitmap(imageUri, this);
                    _imgImage.SetImageBitmap(bitmap);
                    s_image = ImageHelper.CreateImageFileFromUri2(this, imageUri);
                    _txtDeleteImage.Visibility = ViewStates.Visible;

                    _imageManager.UpdateFromURI(imageUri, this);
                }
            }
        }

        public override void OnRequestPermissionsResult(
            int requestCode,
            string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (CameraHelper.CheckCameraPermission(requestCode, grantResults))
            {
                OpenCamera();
            }

            if (GaleryHelper.CheckGaleryPermission(requestCode, grantResults))
            {
                OpenGallery();
            }
        }

        private void GoToCamera()
        {
            if (!CameraHelper.HasCameraPermission(this))
            {
                CameraHelper.RequestCameraPermission(this);
            }
            else
            {
                OpenCamera();
            }
        }

        private void GoToGallery()
        {
            if (!GaleryHelper.HasGaleryPermission(this))
            {
                GaleryHelper.RequestGaleryPermission(this);
            }
            else
            {
                OpenGallery();
            }
        }

        private void OpenCamera()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            //TODO For devices without a camera this could cause a runtime error.
            //if (takePictureIntent.ResolveActivity(PackageManager) != null)
            {
                s_image = ImageHelper.CreateImageFile(this);
                if (s_image != null)
                {
                    var photoURI = FileProvider.GetUriForFile(this, CommonHelper.GetFileProviderAuthorities(this), s_image);
                    intent.PutExtra(MediaStore.ExtraOutput, photoURI);
                    StartActivityForResult(intent, CameraHelper.REQUEST_OPEN_CAMERA);
                }
            }
        }

        private void OpenGallery()
        {
            Intent chooseItemFromGalleryIntent = new Intent(Intent.ActionPick);
            chooseItemFromGalleryIntent.SetType("image/*");
            StartActivityForResult(chooseItemFromGalleryIntent, GaleryHelper.REQUEST_OPEN_GALLERY);
        }

        private void InitializeControls()
        {
            _btnAccept = FindViewById<Button>(Resource.Id.btnAccept_CreateArticle);
            _btnCancel = FindViewById<Button>(Resource.Id.btnCancel_CreateArticle);
            _etName = FindViewById<EditText>(Resource.Id.etName_CreateArticle);
            _etDetails = FindViewById<EditText>(Resource.Id.etDetails_CreateArticle);
            _imgImage = FindViewById<ImageView>(Resource.Id.imgImage_CreateArticle);
            _txtDeleteImage = FindViewById<TextView>(Resource.Id.txtDeleteImage_CreateArticle);
            _spnCategory = FindViewById<Spinner>(Resource.Id.spnCategory_CreateArticle);

            List<string> categoryNames;
            ArrayAdapter adapter;
            if (s_categories.Count > 0)
            {
                categoryNames = s_categories.Select(c => c.Name).ToList();
            }
            else
            {
                categoryNames = new List<string> { "(NO CATEGORIES AVAILABLE)" };
                _spnCategory.Enabled = false;
            }
            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, categoryNames);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _spnCategory.Adapter = adapter;

            _txtDeleteImage.Click += (s, e) =>
            {
                _txtDeleteImage.Visibility = ViewStates.Gone;
                _imageManager.Clear();
                _imgImage.SetImageResource(Resource.Drawable.ic_launcher_foreground);
            };
            _txtDeleteImage.Visibility = ViewStates.Gone;
        }

        public void BindControls()
        {
            _etName.BindProperty(nameof(_etName.Text), _viewModel, nameof(_viewModel.Name));
            _etDetails.BindProperty(nameof(_etDetails.Text), _viewModel, nameof(_viewModel.Details));
            _spnCategory.BindProperty(nameof(_spnCategory.SelectedItemPosition), _viewModel, nameof(_viewModel.SelectedCategoryIndex));
            _imageManager.Control = _imgImage;
            _imageManager.BindProperty(nameof(_imageManager.Path), _viewModel, nameof(_viewModel.ImagePath));
            _imageManager.BindProperty(nameof(_imageManager.Bytes), _viewModel, nameof(_viewModel.ImageData));
            _btnAccept.BindCommand("Click", _viewModel, nameof(_viewModel.CommandSave));
            _btnCancel.BindCommand("Click", _viewModel, nameof(_viewModel.CommandCancel));
            _imgImage.BindCommand("Click", _viewModel, nameof(_viewModel.CommandSelectImage));

            _viewModel.SaveOkEvent += _viewModel_SaveOkEvent;
            _viewModel.EventCancel += _viewModel_CancelEvent;
            _viewModel.EventSelectImage += _viewModel_EventSelectImage;
        }

        private void _viewModel_SaveOkEvent(string sucessfulMsg)
        {
            var toast = Toast.MakeText(this, sucessfulMsg, ToastLength.Short);
            toast.SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal, 0, 0);
            toast.Show();

            SetResult(Result.Ok);
            Finish();
        }

        private void _viewModel_CancelEvent()
        {
            Finish();
        }

        private void _viewModel_EventSelectImage()
        {
            string[] options = { "Take a photo", "Choose from Galery" };
            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            builder.SetTitle("Select option");
            builder.SetItems(
                options,
                (dialog, which) =>
                {
                    switch (which.Which)
                    {
                        case 0:
                            GoToCamera();
                            break;
                        case 1:
                            GoToGallery();
                            break;
                    }
                }
            );
            builder.Show();
        }
    }
}