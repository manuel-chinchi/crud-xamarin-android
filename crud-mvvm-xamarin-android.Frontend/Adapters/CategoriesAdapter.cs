using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Frontend.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Frontend.Adapters
{
    public class CategoriesAdapter : RecyclerView.Adapter
    {
        List<Category> categories;
        List<int> selectedPositions;
        public CategoriesAdapter(List<Category> categories)
        {
            this.categories = categories;
            this.selectedPositions = new List<int>();
        }


        public override int ItemCount => categories.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as CategoryViewHolder;
            viewHolder.Id.Text = categories[position].Id.ToString();
            viewHolder.Name.Text = categories[position].Name;
            viewHolder.Articles.Text = categories[position].ArticleCount.ToString();

            viewHolder.Selected.CheckedChange -= null;
            viewHolder.Selected.Checked = selectedPositions.Contains(holder.Position);
            viewHolder.Selected.CheckedChange += (s, e) =>
            {
                if (e.IsChecked)
                {
                    if (!selectedPositions.Contains(holder.Position))
                        selectedPositions.Add(holder.Position);
                }
                else
                {
                    if (selectedPositions.Contains(holder.Position))
                        selectedPositions.Remove(holder.Position);
                }
                ((CategoriesActivity)holder.ItemView.Context).ToogleDeleteButton(selectedPositions.Count> 0);
            };

        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_category, parent, false);
            return new CategoryViewHolder(view);
        }

        public List<int> GetSelectedPositions()
        {
            selectedPositions.Sort((a, b) => b.CompareTo(a));
            return selectedPositions;
        }

        internal void RemoveAt(int pos)
        {
            categories.RemoveAt(pos);
            NotifyItemRemoved(pos);
        }

        public object GetItemAt(int position)
        {
            return categories[position];
        }

        public void SelectAllItems(bool isSelected)
        {
            selectedPositions.Clear();
            if (isSelected)
            {
                for (int i = 0; i < categories.Count; i++)
                {
                    selectedPositions.Add(i);
                }
            }
            NotifyDataSetChanged();
        }
        public void ClearSelectedPositions()
        {
            selectedPositions.Clear();
        }
        public void UpdateCategories(List<Category> categories)
        {
            this.categories = categories;
        }
    }

    public class CategoryViewHolder : RecyclerView.ViewHolder
    {
        public TextView Id { get; private set; }
        public TextView Name { get; private set; }
        public TextView Articles { get; private set; }
        public CheckBox Selected { get; private set; }

        public CategoryViewHolder(View itemView) : base(itemView)
        {
            Id = ItemView.FindViewById<TextView>(Resource.Id.colIdCategory);
            Name = ItemView.FindViewById<TextView>(Resource.Id.colNameCategory);
            Selected = ItemView.FindViewById<CheckBox>(Resource.Id.chkSelectedCategory);
            Articles = itemView.FindViewById<TextView>(Resource.Id.colArticlesCategory);
        }
    }
}