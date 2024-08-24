﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using crud_xamarin_android.Core.Services;
using crud_xamarin_android.UI.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.UI
{
    [Activity(Label = "CategoryActivity")]
    public class CategoryActivity : Activity
    {
        RecyclerView recyclerView;
        CategoryAdapter adapter;
        CategoryService categoryService;
        public CategoryActivity()
        {
            categoryService = new CategoryService();
            adapter = new CategoryAdapter(categoryService.GetCategories());
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_category);

            recyclerView = FindViewById<RecyclerView>(Resource.Id.lstCategories);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));
            recyclerView.SetAdapter(adapter);

            var btnAddCategory = FindViewById<Button>(Resource.Id.btnAddCategory);
            btnAddCategory.Click += BtnAddCategory_Click;
            var btnDeleteCategory = FindViewById<Button>(Resource.Id.btnDeleteCategory);
            btnDeleteCategory.Click += BtnDeleteCategory_Click;

            var chkSelectAllCategories = FindViewById<CheckBox>(Resource.Id.chkSelectAllCategories);
            chkSelectAllCategories.CheckedChange += ChkSelectAllCategories_CheckedChange;

        }

        private void ChkSelectAllCategories_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                adapter.SelectAllItems(true);
            }
            else
            {
                adapter.SelectAllItems(false);
            }
        }

        private void BtnAddCategory_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(CreateCategoryActivity));
            StartActivity(intent);
        }

        private void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            var builder = new AlertDialog.Builder(this);
            builder.SetTitle("Delete");
            builder.SetMessage("Are you sure you want to delete the selected categories?");
            builder.SetPositiveButton("Yes", (senderAlert, args) =>
            {
                DeleteCategory();
            });

            builder.SetNegativeButton("No", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Cancel action" , ToastLength.Short).Show();
            });

            var alertDialog = builder.Create();
            alertDialog.Show();
        }

        private void DeleteCategory()
        {
            var positions = adapter.GetSelectedPositions();
            foreach (var pos in positions)
            {
                adapter.RemoveAt(pos);
            }

            ToogleCheckHeader(false);
        }

        public void ToogleDeleteButton(bool isAnySelected)
        {
            var btnDeleteCategory = FindViewById<Button>(Resource.Id.btnDeleteCategory);
            btnDeleteCategory.Enabled = isAnySelected;
        }

        public void ToogleCheckHeader(bool isChecked)
        {
            var chkSelectAllCategories = FindViewById<CheckBox>(Resource.Id.chkSelectAllCategories);
            chkSelectAllCategories.Checked = isChecked;
        }
    }
}