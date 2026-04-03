using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using crud_xamarin_android.Backend.ViewModels;
using crud_xamarin_android.Frontend.Activities.Contracts;
using crud_xamarin_android.Frontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Activities
{
    [Activity(Label = "")]
    public class CreateCategoryActivity : AppCompatActivity, IBaseActivity
    {
        private Button _btnAccept;
        private Button _btnCancel;
        private EditText _etName;

        private readonly CreateCategoryViewModel _viewModel;

        public CreateCategoryActivity()
        {
            _viewModel = new CreateCategoryViewModel();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_create_category);

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

        private void InitializeControls()
        {
            _btnAccept = FindViewById<Button>(Resource.Id.btnAccept_CreateCategory);
            _btnCancel = FindViewById<Button>(Resource.Id.btnCancel_CreateCategory);
            _etName = FindViewById<EditText>(Resource.Id.etName_CreateCategory);
        }

        public void BindControls()
        {
            _etName.BindProperty(nameof(_etName.Text), _viewModel, nameof(_viewModel.Name));
            _btnAccept.BindCommand("Click", _viewModel, nameof(_viewModel.SaveCommand));
            _btnCancel.BindCommand("Click", _viewModel, nameof(_viewModel.CancelCommand));

            _viewModel.SaveOkEvent += _viewModel_SaveOkEvent;
            _viewModel.CancelOkEvent += _viewModel_CancelOkEvent;
        }

        private void _viewModel_SaveOkEvent(string sucessfulMsg)
        {
            var toast = Toast.MakeText(this, sucessfulMsg, ToastLength.Short);
            toast.SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal, 0, 0);
            toast.Show();

            SetResult(Result.Ok);
            Finish();
        }

        private void _viewModel_CancelOkEvent()
        {
            Finish();
        }
    }
}