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
    public class CategoriesViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;

        public IEnumerable<Category> Categories
        {
            get => _categoryService.GetCategories();
        }

        public ICommand DeleteCommand { get; }
        public ICommand ShowDialogDeleteCommand { get; }
        public ICommand ShowDialogAddCommand { get; }

        public event Action ShowDialogAddEvent;
        public event Action ShowDialogDeleteEvent;

        public CategoriesViewModel()
        {
            _categoryService = new CategoryService();

            ShowDialogDeleteCommand = new Command(ShowDialogDelete);
            ShowDialogAddCommand = new Command(ShowDialogAdd);
            DeleteCommand = new Command<int>(Delete);
        }

        private void ShowDialogAdd()
        {
            ShowDialogAddEvent?.Invoke();
        }

        private void ShowDialogDelete()
        {
            ShowDialogDeleteEvent?.Invoke();
        }

        private void Delete(int id)
        {
            _categoryService.DeleteCategory(id);
        }
    }
}