using System;
using crud_xamarin_android.Backend.Models;
using crud_xamarin_android.Backend.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace crud_xamarin_android.Backend.ViewModels
{
    public class CreateCategoryViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private Category _category;

        public string Name
        {
            get => _category.Name;
            set
            {
                _category.Name = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event Action<string> SaveOkEvent;
        public event Action CancelOkEvent;

        public CreateCategoryViewModel()
        {
            _category = new Category();
            _categoryService = new CategoryService();

            SaveCommand = new Command(Save);
            CancelCommand = new Command(Cancel);
        }

        private void Save()
        {
            _categoryService.AddCategory(_category);
            SaveOkEvent?.Invoke("Category created successfully!");
        }

        private void Cancel()
        {
            CancelOkEvent?.Invoke();
        }
    }
}