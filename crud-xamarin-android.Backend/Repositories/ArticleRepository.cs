using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Backend.Repositories
{
    internal class ArticleRepository : BaseRepository, IArticleRepository
    {
        public ArticleRepository()
        {
        }

        private void checkTablesExist()
        {
            Connection.CreateTable<Category>();
            Connection.CreateTable<Article>();
        }

        public void Delete(int id)
        {
            checkTablesExist();
            Connection.Delete<Article>(id);
        }

        public IEnumerable<Article> GetAll()
        {
            checkTablesExist();
            var articles = Connection.Table<Article>().ToList();
            return articles;
        }

        public Article GetById(int id)
        {
            checkTablesExist();
            var article = Connection.Find<Article>(id);
            return article ?? null;
        }

        public void Insert(Article article)
        {
            checkTablesExist();
            Connection.Insert(article);
        }

        public int Update(Article article)
        {
            checkTablesExist();
            return Connection.Update(article, typeof(Article));
        }
    }
}