using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace crud_xamarin_android.Backend.ViewModels
{
    public class EditArticleViewModel : BaseViewModel
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

        public ICommand UpdateCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectImageCommand { get; }
        public ICommand LoadArticleByIdCommand { get; }

        public event Action CancelEvent;
        public event Action SelectImageEvent;
        public event Action<string> UpdateOkEvent;

        public EditArticleViewModel()
        {
            _article = new Article();
            _articleService = new ArticleService();
            _categoryService = new CategoryService();
            s_categories = _categoryService.GetCategories();

            LoadArticleByIdCommand = new Command<int>(LoadArticleById);
            UpdateCommand = new Command<int>(Update);
            CancelCommand = new Command(Cancel);
            SelectImageCommand = new Command(SelectImage);
        }

        private void LoadArticleById(int id)
        {
            var article = _articleService.GetArticleById(id);
            Name = article.Name;
            Details = article.Details;
            ImagePath = article.ImagePath;
            ImageData = article.ImageData;
            /// TODO falta setear el indice de categoria en el Spinner
        }

        private void SelectImage(object obj)
        {
            SelectImageEvent?.Invoke();
        }

        private void Cancel(object obj)
        {
            CancelEvent?.Invoke();
        }

        private void Update(int id)
        {
            var article = _articleService.GetArticleById(id);
            var category = _categoryService.GetCategories().ToList()[SelectedCategoryIndex];

            article.Name = Name;
            article.Details = Details;
            article.ImagePath = ImagePath;
            article.ImageData = ImageData;

            /// TODO revisar forma de actualizacion de Categoria en Article y carga 
            /// a veces me confundo creyendo que seteando el objeto Category ya se setea
            /// la categoria en si
            if (category != null)
                article.CategoryId = category.Id;

            _articleService.UpdateArticle(article);

            UpdateOkEvent?.Invoke("Article successfully updated!");
        }
    }
}
// como me duele la espalda :p ouch!!!