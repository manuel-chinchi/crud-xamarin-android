using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Services;
using crud_xamarin_android.Backend.Helpers;

namespace crud_xamarin_android.Backend.ViewModels
{
    public class CreateArticleViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;
        private readonly CategoryService _categoryService;
        private static IEnumerable<Category> s_categories;
        private Article _article;
        private int _selectedCategoryIndex;

        public string Name
        {
            get => _article.Name;
            set
            {
                _article.Name = value;
                OnPropertyChanged();
            }
        }

        public string Details
        {
            get => _article.Details;
            set
            {
                _article.Details = value;
                OnPropertyChanged();
            }
        }

        public int SelectedCategoryIndex
        {
            get => _selectedCategoryIndex;
            set
            {
                _selectedCategoryIndex = value;
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => _article.ImagePath;
            set
            {
                _article.ImagePath = value;
                OnPropertyChanged();
            }
        }

        public byte[] ImageData
        {
            get => _article.ImageData;
            set
            {
                _article.ImageData = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Category> Categories
        {
            get => s_categories;
        }

        public ICommand CommandSave => new Command(Save);
        public ICommand CommandCancel => new Command(Cancel);
        public ICommand CommandSelectImage => new Command(SelectImage);

        public event Action<string> SaveOkEvent;
        public event Action EventCancel;
        public event Action EventSelectImage;
        /// TODO mejoras a futuro
        /// - Eventos para capturar errores u otros, x ej. SaveErrEvent, SaveExceptEvent, etc.

        public CreateArticleViewModel()
        {
            _article = new Article();
            _articleService = new ArticleService();
            _categoryService = new CategoryService();
            s_categories = _categoryService.GetCategories().OrderBy(c => c.Name).ToList();
        }

        private void Save()
        {
            var categories = _categoryService.GetCategories();
            if (categories.Count()==0)
            {
                _articleService.AddArticle(new Article
                {
                    Category = new Category()
                    {
                        Id = CategoryHelper.ID_EMPTY_CATEGORY,
                        Name = CategoryHelper.NAME_EMPTY_CATEGORY
                    },
                    CategoryId = CategoryHelper.ID_EMPTY_CATEGORY
                });
            }
            else
            {
                Category category = categories.OrderBy(c => c.Name).ToList()[SelectedCategoryIndex];
                _article.Category = category;
                _article.CategoryId = category.Id;
                _articleService.AddArticle(_article);
            }

            SaveOkEvent?.Invoke("Article created successfully!");
        }

        private void Cancel()
        {
            EventCancel?.Invoke();
        }

        private void SelectImage()
        {
            EventSelectImage?.Invoke();
        }
    }
}