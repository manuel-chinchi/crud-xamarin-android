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
    internal class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository()
        {
        }

        private void checkTablesExist()
        {
            Connection.CreateTable<Category>();
        }

        public void Delete(int id)
        {
            checkTablesExist();
            Connection.Delete<Category>(id);
        }

        public IEnumerable<Category> GetAll()
        {
            checkTablesExist();
            var categories = Connection.Table<Category>().ToList();
            return categories;
        }

        public Category GetById(int id)
        {
            checkTablesExist();
            var category = Connection.Find<Category>(id);
            return category;
        }

        public void Insert(Category category)
        {
            checkTablesExist();
            Connection.Insert(category);
        }

        public int Update(Category category)
        {
            checkTablesExist();
            return Connection.Update(category, typeof(Category));
        }
    }
}