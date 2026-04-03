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
    public class ArticlesViewModel : BaseViewModel
    {
        private readonly ArticleService _articleService;

        public IEnumerable<Article> Articles
        {
            get => _articleService.GetArticles();
        }

        public ICommand DeleteCommand { get; }
        public ICommand ShowDialogDeleteCommand { get; }
        public ICommand ShowDialogAddCommand { get; }
        public ICommand ShowDialogEditCommand { get; }

        public event Action ShowDialogDeleteEvent;
        public event Action ShowDialogAddEvent;
        public event Action ShowDialogEditEvent;
        
        public ArticlesViewModel()
        {
            _articleService = new ArticleService();

            ShowDialogAddCommand = new Command(ShowDialogAdd);
            ShowDialogEditCommand = new Command(ShowDialogEdit);
            ShowDialogDeleteCommand = new Command(ShowDialogDelete);
            DeleteCommand = new Command<int>(Delete);
        }

        private void ShowDialogAdd()
        {
            ShowDialogAddEvent?.Invoke();
        }

        private void ShowDialogEdit()
        {
            ShowDialogEditEvent?.Invoke();
        }

        private void ShowDialogDelete()
        {
            ShowDialogDeleteEvent?.Invoke();
        }

        private void Delete(int id)
        {
            _articleService.DeleteArticle(id);
        }
    }
}