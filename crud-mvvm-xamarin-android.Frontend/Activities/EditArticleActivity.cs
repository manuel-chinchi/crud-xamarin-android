using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using crud_xamarin_android.Backend.Helpers;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Services;
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
    public class EditArticleActivity : AppCompatActivity, IBaseActivity
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
        private Article article;
        private readonly EditArticleViewModel _viewModel;
        private readonly ObservableImageManager _imageManager;

        private static int s_articleId;
        private static int s_categoryId;

        public EditArticleActivity()
        {
            _viewModel = new EditArticleViewModel();
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

            s_articleId = Intent.GetIntExtra("ArticleId", -1);
            s_categoryId = Intent.GetIntExtra("CategoryId", -1);

            LoadArticle(s_articleId);
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
                _txtDeleteImage.Visibility = ViewStates.Gone;

                _imageManager.UpdateFromFile(s_image);
            }

            if (GaleryHelper.CheckResultGalery(requestCode, resultCode))
            {
                if (data != null)
                {
                    // TODO si la imagen es muy grande da error
                    //imgArticle.SetImageURI(data.Data);
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
            Intent takePictureIntent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
            //TODO For devices without a camera this could cause a runtime error.
            //if (takePictureIntent.ResolveActivity(PackageManager) != null)
            {
                s_image = ImageHelper.CreateImageFile(this);
                if (s_image != null)
                {
                    var photoURI = AndroidX.Core.Content.FileProvider.GetUriForFile(this, CommonHelper.GetFileProviderAuthorities(this), s_image);
                    takePictureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);
                    StartActivityForResult(takePictureIntent, CameraHelper.REQUEST_OPEN_CAMERA);
                }
            }
        }

        private void OpenGallery()
        {
            Intent chooseItemFromGalleryIntent = new Intent(Intent.ActionPick);
            chooseItemFromGalleryIntent.SetType("image/*");
            StartActivityForResult(chooseItemFromGalleryIntent, GaleryHelper.REQUEST_OPEN_GALLERY);
        }

        private void LoadArticle(int id)
        {
            _viewModel.LoadArticleByIdCommand.Execute(id);

            //_imageManager.Bytes = _viewModel.ImageData;
            _imageManager.Path = _viewModel.ImagePath;

            if (string.IsNullOrEmpty(_imageManager.Path))
            {
                _imgImage.SetImageResource(Resource.Drawable.ic_launcher_foreground);
                _txtDeleteImage.Visibility = ViewStates.Invisible;
            }

            List<string> categoryNames;
            ArrayAdapter adapter;

            if (article != null)
            {
                _etName.Text = article.Name;
                _etDetails.Text = article.Details;

                if (article.ImageData != null)
                {
                    // TODO con imagenes muy grandes da error
                    //var bitmap = BitmapFactory.DecodeFile(article.ImagePath);
                    var bitmap = ImageHelper.GetResizedBitmapFromBytes(article.ImageData, 1024, 1024);
                    _imgImage.SetImageBitmap(bitmap);
                    _txtDeleteImage.Visibility = ViewStates.Visible;
                }
                else
                {
                    _txtDeleteImage.Visibility = ViewStates.Gone;
                }
            }

            if (s_categoryId == CategoryHelper.ID_EMPTY_CATEGORY && s_categories.Count > 0)
            {
                s_categories.Insert(0, new Category
                {
                    Id = CategoryHelper.ID_EMPTY_CATEGORY,
                    Name = CategoryHelper.NAME_EMPTY_CATEGORY
                });
                categoryNames = s_categories.Select(c => c.Name).ToList();
                adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, categoryNames);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                _spnCategory.Adapter = adapter;
                int position = s_categories.FindIndex(c => c.Id == s_categoryId);
                _spnCategory.SetSelection(position);
            }
            else if (s_categories.Count > 0)
            {
                categoryNames = s_categories.Select(c => c.Name).ToList();
                adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, categoryNames);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                _spnCategory.Adapter = adapter;
                int position = s_categories.FindIndex(c => c.Id == s_categoryId);
                _spnCategory.SetSelection(position);
            }
            else
            {
                categoryNames = new List<string> { CategoryHelper.NAME_EMPTY_CATEGORY };
                adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, categoryNames);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                _spnCategory.Adapter = adapter;
                _spnCategory.Enabled = false;
            }
        }

        private void InitializeControls()
        {
            _etName = FindViewById<EditText>(Resource.Id.etName_CreateArticle);
            _etDetails = FindViewById<EditText>(Resource.Id.etDetails_CreateArticle);
            _spnCategory = FindViewById<Spinner>(Resource.Id.spnCategory_CreateArticle);
            _imgImage = FindViewById<ImageView>(Resource.Id.imgImage_CreateArticle);
            _txtDeleteImage = FindViewById<TextView>(Resource.Id.txtDeleteImage_CreateArticle);
            _btnAccept = FindViewById<Button>(Resource.Id.btnAccept_CreateArticle);
            _btnCancel = FindViewById<Button>(Resource.Id.btnCancel_CreateArticle);

            _txtDeleteImage.Click += (s, e) =>
            {
                _txtDeleteImage.Visibility = ViewStates.Invisible;
                ///TODO importante!! .Clear() tambien ejecuta .SetImageResource() por ende se debe llamar
                ///luego de ejecutar este (Clear()) sino no se actualizara el control ImageView
                ///sino tambien se podría pasar un parametro al metodo así .Clear(updateBindedControl:false)
                ///pero sería raro ya que justamente el manager se creo para eso. pensarlo...
                _imageManager.Clear();
                _imgImage.SetImageResource(Resource.Drawable.ic_launcher_foreground);
            };
        }

        public void BindControls()
        {
            _etName.BindProperty(nameof(_etName.Text), _viewModel, nameof(_viewModel.Name));
            _etDetails.BindProperty(nameof(_etDetails.Text), _viewModel, nameof(_viewModel.Details));
            _spnCategory.BindProperty(nameof(_spnCategory.SelectedItemPosition), _viewModel, nameof(_viewModel.SelectedCategoryIndex));
            _imageManager.Control = _imgImage;
            _imageManager.BindProperty(nameof(_imageManager.Path), _viewModel, nameof(_viewModel.ImagePath));
            _imageManager.BindProperty(nameof(_imageManager.Bytes), _viewModel, nameof(_viewModel.ImageData));
            _imgImage.BindCommand("Click", _viewModel, nameof(_viewModel.SelectImageCommand));
            _btnAccept.BindCommandWithParams<int>("Click", _viewModel, nameof(_viewModel.UpdateCommand),
                () => s_articleId);
            _btnCancel.BindCommand("Click", _viewModel, nameof(_viewModel.CancelCommand));

            _viewModel.UpdateOkEvent += _viewModel_UpdateOkEvent;
            _viewModel.CancelEvent += _viewModel_CancelEvent;
            _viewModel.SelectImageEvent += _viewModel_SelectImageEvent;
        }

        private void _viewModel_SelectImageEvent()
        {
            string[] options = { "Take a photo", "Choose from Galery" };
            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            builder.SetTitle("Select option");
            builder.SetItems(options, (dialog, which) =>
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
            });
            builder.Show();
        }

        private void _viewModel_UpdateOkEvent(string msg)
        {
            var toast = Toast.MakeText(this, msg, ToastLength.Short);
            toast.SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal, 0, 0);
            toast.Show();

            Intent resultIntent = new Intent();
            SetResult(Result.Ok, resultIntent);
            Finish();
        }

        private void _viewModel_CancelEvent()
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }
}