using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AndroidX.AppCompat.App;
using crud_xamarin_android.Frontend.Adapters;
using crud_xamarin_android.Frontend.Activities.Contracts;
using crud_xamarin_android.Backend.ViewModels;
using crud_xamarin_android.Frontend.Models;

namespace crud_xamarin_android.Frontend.Activities
{
    [Activity(Label = "Articles")]
    public class ArticlesActivity : AppCompatActivity, IBaseActivity
    {
        private Button _btnAdd;
        private Button _btnEdit;
        private Button _btnDelete;
        private CheckBox _chkSelectAll;
        private RecyclerView _rvArticles;
        private ArticlesAdapter _adapter;
        private readonly ArticlesViewModel _viewModel;

        public ArticlesActivity()
        {
            _viewModel = new ArticlesViewModel();
            _adapter = new ArticlesAdapter(_viewModel.Articles.ToList());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_articles);

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

            if (requestCode == 1 && resultCode == Result.Ok)
            {
                _adapter.UpdateArticles(_viewModel.Articles.ToList());
                _adapter.ClearSelectedPositions();
                _adapter.NotifyDataSetChanged();
                _btnEdit.Enabled = false;
                _btnDelete.Enabled = false;
            }

            if (requestCode == 1000 && resultCode == Result.Ok)
            {
                _adapter.UpdateArticles(_viewModel.Articles.ToList());
                _adapter.NotifyDataSetChanged();
            }
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

        private void _viewModel_ShowDialogEditEvent()
        {
            if (_adapter.GetSelectedPositions().Count == 1)
            {
                int position = _adapter.GetSelectedPositions()[0];
                var article = _adapter.GetArticleAt(position);

                var intent = new Intent(this, typeof(EditArticleActivity));
                intent.PutExtra("ArticleId", article.Id);
                // TODO check case 'null Category'!!
                intent.PutExtra("CategoryId", article.Category.Id);
                StartActivityForResult(intent, 1);
            }
        }

        private void _viewModel_ShowDialogDeleteEvent()
        {
            var builder = new AndroidX.AppCompat.App.AlertDialog.Builder(this);
            builder.SetTitle(Resource.String.title_delete);
            builder.SetMessage(Resource.String.message_warning_article_delete);
            builder.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                DeleteArticle();
            });
            builder.SetNegativeButton("No", (senderAlert, args) =>
            {
                Toast.MakeText(this, Resource.String.message_cancel, ToastLength.Short).Show();
            });

            var alertDialog = builder.Create();
            alertDialog.Show();
        }

        private void _viewModel_ShowDialogAddEvent()
        {
            var intent = new Intent(this, typeof(CreateArticleActivity));
            StartActivityForResult(intent, 1000);
        }

        private void DeleteArticle()
        {
            var positions = _adapter.GetSelectedPositions();

            foreach (var pos in positions)
            {
                _viewModel.DeleteCommand.Execute(_adapter.GetArticleAt(pos).Id);
                _adapter.RemoveAt(pos);
            }

            _adapter.UpdateArticles(_viewModel.Articles.ToList());
            _adapter.ClearSelectedPositions();
            ToggleDeleteButton(false);
            ToogleCheckHeader(false);
        }

        public void ToggleDeleteButton(bool isAnySelected)
        {
            _btnDelete.Enabled = isAnySelected;
        }

        public void ToggleEditButton(bool isOneItemSelected)
        {
            _btnEdit.Enabled = isOneItemSelected;
        }

        private void ToogleCheckHeader(bool isChecked)
        {
            _chkSelectAll.Checked = false;
        }

        private void InitializeControls()
        {
            _btnAdd = FindViewById<Button>(Resource.Id.btnAdd_Articles);
            _btnEdit = FindViewById<Button>(Resource.Id.btnEdit_Articles);
            _btnEdit.Enabled = false;
            _btnDelete = FindViewById<Button>(Resource.Id.btnDelete_Articles);
            _btnDelete.Enabled = false;
            _rvArticles = FindViewById<RecyclerView>(Resource.Id.rvArticles_Articles);
            _rvArticles.SetLayoutManager(new LinearLayoutManager(this));
            _rvArticles.SetAdapter(_adapter);
            _chkSelectAll = FindViewById<CheckBox>(Resource.Id.cbSelectAll_Articles);
            _chkSelectAll.CheckedChange += ChkSelectAll_CheckedChange;
        }

        public void BindControls()
        {
            _viewModel.ShowDialogAddEvent += _viewModel_ShowDialogAddEvent;
            _viewModel.ShowDialogEditEvent += _viewModel_ShowDialogEditEvent;
            _viewModel.ShowDialogDeleteEvent += _viewModel_ShowDialogDeleteEvent;

            _btnAdd.BindCommand(eventName: "Click", _viewModel, nameof(_viewModel.ShowDialogAddCommand));
            _btnEdit.BindCommand(eventName: "Click", _viewModel, nameof(_viewModel.ShowDialogEditCommand));
            _btnDelete.BindCommand(eventName: "Click", _viewModel, nameof(_viewModel.ShowDialogDeleteCommand));
        }
    }
}