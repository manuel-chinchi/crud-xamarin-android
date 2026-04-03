using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Backend.Repositories.Contracts
{
    internal interface IBaseRepository<T>
    {
        void Insert(T item);
        int Update(T item);
        void Delete(int id);
        IEnumerable<T> GetAll();
        T GetById(int id);
    }
}