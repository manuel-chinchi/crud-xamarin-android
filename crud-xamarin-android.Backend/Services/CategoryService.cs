using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using crud_xamarin_android.Backend.Helpers;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Repositories;
using crud_xamarin_android.Backend.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Backend.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IArticleRepository _articleRepository;
        private Category untrackedCategory;

        public CategoryService()
        {
            _categoryRepository = new CategoryRepository();
            _articleRepository = new ArticleRepository();
            untrackedCategory = new Category
            {
                Id = CategoryHelper.ID_EMPTY_CATEGORY,
                Name = CategoryHelper.NAME_EMPTY_CATEGORY
            };
        }

        public IEnumerable<Category> GetCategories()
        {
            var categories = _categoryRepository.GetAll().ToList();
            var articles = _articleRepository.GetAll();

            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].ArticleCount > 0)
                {
                    categories[i].Articles = articles.Where(a => a.CategoryId == categories[i].Id).ToList();
                }
            }

            return categories;
        }

        public Category GetCategoryById(int id)
        {
            var category = _categoryRepository.GetById(id);
            var articles = _articleRepository.GetAll();
            category.Articles = articles.Where(a => a.CategoryId == category.Id).ToList();
            return category;
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.Insert(category);
        }

        public void UpdateCategory(Category category)
        {
            _categoryRepository.Update(category);
        }

        public void DeleteCategory(int id)
        {
            var category = _categoryRepository.GetById(id);
            var articles = _articleRepository.GetAll().Where(a => a.CategoryId == category.Id).ToList();

            if (articles != null && articles.Count > 0)
            {
                foreach (var article in articles)
                {
                    article.CategoryId = untrackedCategory.Id;
                    _articleRepository.Update(article);
                }
            }

            _categoryRepository.Delete(id);
        }
    }
}