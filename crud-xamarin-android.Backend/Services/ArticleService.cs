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
    public class ArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICategoryRepository _categoryRepository;
        private Category untrackedCategory;

        public ArticleService()
        {
            _articleRepository = new ArticleRepository();
            _categoryRepository = new CategoryRepository();
            untrackedCategory = new Category
            {
                Id = CategoryHelper.ID_EMPTY_CATEGORY,
                Name = CategoryHelper.NAME_EMPTY_CATEGORY
            };
        }

        public IEnumerable<Article> GetArticles()
        {
            var articles = _articleRepository.GetAll().ToList();

            for (int i = 0; i < articles.Count; i++)
            {
                articles[i].Category = _categoryRepository.GetById(articles[i].CategoryId);

                if (articles[i].Category == null)
                {
                    articles[i].Category = new Category
                    {
                        Id = CategoryHelper.ID_EMPTY_CATEGORY,
                        Name = CategoryHelper.NAME_EMPTY_CATEGORY
                    };
                }
            }

            return articles;
        }

        public Article GetArticleById(int id)
        {
            var article = _articleRepository.GetById(id);
            var category = _categoryRepository.GetById(article.CategoryId);

            if (category != null)
            {
                article.Category = category;
            }

            return article;
        }

        public void AddArticle(Article article)
        {
            var category = _categoryRepository.GetById(article.CategoryId);

            if (category != null)
            {
                category.ArticleCount++;
                _categoryRepository.Update(category);
            }

            _articleRepository.Insert(article);
        }

        public void DeleteArticle(int id)
        {
            var article = _articleRepository.GetById(id);
            var category = _categoryRepository.GetById(article.CategoryId);

            if (category != null)
            {
                category.ArticleCount--;
                _categoryRepository.Update(category);
            }

            _articleRepository.Delete(id);
        }

        public void UpdateArticle(Article article)
        {
            var oldArticle = _articleRepository.GetById(article.Id);

            if (oldArticle.CategoryId != article.CategoryId)
            {
                if (article.CategoryId != untrackedCategory.Id)
                {
                    var oldCategory = _categoryRepository.GetById(oldArticle.CategoryId);
                    if (oldCategory != null)
                    {
                        oldCategory.ArticleCount--;
                        _categoryRepository.Update(oldCategory);
                    }
                }

                var category = _categoryRepository.GetById(article.CategoryId);
                category.ArticleCount++;
                _categoryRepository.Update(category);
            }

            _articleRepository.Update(article);
        }
    }
}