using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.ViewModels;
using crud_xamarin_android.Frontend.Activities.Contracts;
using crud_xamarin_android.Frontend.Adapters;
using crud_xamarin_android.Frontend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Activities
{
    [Activity(Label = "Categories")]
    public class CategoriesActivity : AppCompatActivity, IBaseActivity
    {
        private Button _btnAdd;
        private Button _btnDelete;
        private CheckBox _chkSelectAll;
        private RecyclerView _rvCategories;
        private CategoriesAdapter _adapter;
        private readonly CategoriesViewModel _viewModel;

        public CategoriesActivity()
        {
            _viewModel = new CategoriesViewModel();
            _adapter = new CategoriesAdapter(_viewModel.Categories.ToList());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_categories);

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

            if (requestCode == 1000 && resultCode == Result.Ok)
            {
                //adapter.UpdateCategories(_categoryService.GetCategories().ToList());
                _adapter.UpdateCategories(_viewModel.Categories.ToList());
                _adapter.NotifyDataSetChanged();
            }
        }

        internal void ToogleDeleteButton(bool isAnySelected)
        {
            _btnDelete.Enabled = isAnySelected ? true : false;
        }

        private void ChkSelectAll_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                _adapter.SelectAllItems(true);
            }
            else
            {
                _adapter.SelectAllItems(false);
            }
        }

        private void _viewModel_ShowDialogAddEvent()
        {
            var intent = new Intent(this, typeof(CreateCategoryActivity));
            StartActivityForResult(intent, 1000);
        }

        private void _viewModel_ShowDialogDeleteEvent()
        {
            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.title_delete);
            builder.SetMessage(Resource.String.message_delete_category);
            builder.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                ConfirmOrCancelDeleteCategory();
            });
            builder.SetNegativeButton("No", (senderAlert, args) =>
            {
                Toast.MakeText(this, Resource.String.message_cancel, ToastLength.Short).Show();
            });

            var alertDialog = builder.Create();
            alertDialog.Show();
        }

        private void ConfirmOrCancelDeleteCategory()
        {
            bool hasRelatedArticles = false;
            var positions = _adapter.GetSelectedPositions();

            foreach (var pos in positions)
            {
                var category = (Category)_adapter.GetItemAt(pos);
                if (category.ArticleCount != 0)
                {
                    hasRelatedArticles = true;
                    continue;
                }
            }

            if (hasRelatedArticles)
            {
                var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
                builder.SetTitle(Resource.String.title_warning);
                builder.SetMessage(Resource.String.message_warning_delete_category);
                builder.SetPositiveButton("Yes", (sender, args) =>
                {
                    DeleteCategory();
                });
                builder.SetNegativeButton("No", (sender, args) =>
                {
                });

                var alertDialog = builder.Create();
                alertDialog.Show();
            }
            else
            {
                DeleteCategory();
            }
        }

        private void DeleteCategory()
        {
            var positions = _adapter.GetSelectedPositions();

            foreach (var pos in positions)
            {
                var categoryId = ((Category)_adapter.GetItemAt(pos)).Id;
                _viewModel.DeleteCommand.Execute(categoryId);
                _adapter.RemoveAt(pos);
            }

            _adapter.UpdateCategories(_viewModel.Categories.ToList());
            _adapter.ClearSelectedPositions();
            _btnDelete.Enabled = false;
            _chkSelectAll.Checked = false;
        }

        private void InitializeControls()
        {
            _btnAdd = FindViewById<Button>(Resource.Id.btnAdd_Categories);
            _btnDelete = FindViewById<Button>(Resource.Id.btnDelete_Categories);
            _btnDelete.Enabled = false;
            _rvCategories = FindViewById<RecyclerView>(Resource.Id.rvCategories_Categories);
            _rvCategories.SetLayoutManager(new LinearLayoutManager(this));
            _rvCategories.SetAdapter(_adapter);
            _chkSelectAll = FindViewById<CheckBox>(Resource.Id.cbSelectAll_Categories);
            _chkSelectAll.CheckedChange += ChkSelectAll_CheckedChange;
        }

        public void BindControls()
        {
            _viewModel.ShowDialogDeleteEvent += _viewModel_ShowDialogDeleteEvent;
            _viewModel.ShowDialogAddEvent += _viewModel_ShowDialogAddEvent;

            _btnAdd.BindCommand("Click", _viewModel, nameof(_viewModel.ShowDialogAddCommand));
            _btnDelete.BindCommand("Click", _viewModel, nameof(_viewModel.ShowDialogDeleteCommand));
        }
    }
}