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
    public class ArticlesAdapter : RecyclerView.Adapter
    {
        List<Article> articles;
        List<int> selectedPositions;

        public ArticlesAdapter(List<Article> articles)
        {
            this.articles = articles;
            this.selectedPositions = new List<int>();
        }

        public override int ItemCount => articles.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as ArticleViewHolder;
            viewHolder.Name.Text = articles[position].Name;
            viewHolder.Details.Text = articles[position].Details;
            viewHolder.Id.Text = articles[position].Id.ToString();
            viewHolder.Category.Text = articles[position].Category.Name;

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
                ((ArticlesActivity)holder.ItemView.Context).ToggleDeleteButton(selectedPositions.Count > 0);
                ((ArticlesActivity)holder.ItemView.Context).ToggleEditButton(selectedPositions.Count == 1);
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_article, parent, false);
            return new ArticleViewHolder(view);
        }

        public List<int> GetSelectedPositions()
        {
            // NOTE 23.08.24:If the list of positions ordered from largest to smallest is not returned, deleting rows will present problems
            selectedPositions.Sort((a, b) => b.CompareTo(a));
            return selectedPositions;
        }

        public void SelectAllItems(bool isSelected)
        {
            selectedPositions.Clear();

            if (isSelected)
            {
                for (int i = 0; i < articles.Count; i++)
                {
                    selectedPositions.Add(i);
                }
            }

            // Notifica al adaptador que los datos han cambiado
            NotifyDataSetChanged();
        }

        internal void UpdateArticles(List<Article> articles)
        {
            this.articles = articles;
        }

        internal Article GetArticleAt(int position)
        {
            return articles[position];
        }

        internal void ClearSelectedPositions()
        {
            selectedPositions.Clear();
        }

        internal void RemoveAt(int position)
        {
            articles.RemoveAt(position);
            NotifyItemRemoved(position);
        }
    }

    public class ArticleViewHolder : RecyclerView.ViewHolder
    {
        public TextView Id { get; private set; }
        public TextView Name { get; private set; }
        public TextView Details { get; private set; }
        public TextView Category { get; private set; }
        public CheckBox Selected { get; private set; }

        public ArticleViewHolder(View itemView):base(itemView)
        {
            Id = itemView.FindViewById<TextView>(Resource.Id.colId);
            Name = itemView.FindViewById<TextView>(Resource.Id.colName);
            Details = itemView.FindViewById<TextView>(Resource.Id.colDetails);
            Category = ItemView.FindViewById<TextView>(Resource.Id.colCategory);
            Selected = ItemView.FindViewById<CheckBox>(Resource.Id.chkSelectedArticle);
        }
    }
}