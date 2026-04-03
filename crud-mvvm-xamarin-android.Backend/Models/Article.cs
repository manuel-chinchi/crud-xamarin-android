using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Backend.Models
{
    public class Article
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        [Ignore]
        public Category Category { get; set; }
        public int CategoryId { get; set; } // foreign key
        public string ImagePath { get; set; }
        public byte[] ImageData { get; set; }
    }
}