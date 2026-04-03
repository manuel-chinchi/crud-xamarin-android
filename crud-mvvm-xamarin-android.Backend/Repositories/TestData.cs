using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using crud_xamarin_android.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace crud_xamarin_android.Backend.Repositories
{
    public class TestData
    {
        public IEnumerable<Article> GetArticles()
        {
            var articles = new List<Article>()
            {
                new Article{Id=1, Name="zapatilla", Details="talle 40, cuero", Category = new Category { Id = 1, Name="Otro"} , CategoryId=1},
                new Article{Id=2, Name="zapatilla", Details="talle 42, cuero, mujer", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=3, Name="camisa", Details="talle L", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=4, Name="camisa", Details="talle M, lisa", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=5, Name="jean", Details="talle 41-43 chupin", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=6, Name="gorra", Details="varios colores lisas", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=7, Name="medias", Details="medianas", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=8, Name="zapato", Details="unisex, talle 40, sintético", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=9, Name="zapato", Details="talle 48", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
                new Article{Id=10, Name="gorra", Details="lisa blanca", Category = new Category { Id = 1, Name="Otro"}, CategoryId=1},
            };
            return articles;
        }

        public IEnumerable<Category> GetCategories()
        {
            var categories = new List<Category>()
            {
                new Category { Id = 1, Name = "Otro", ArticleCount = GetArticles().Count() },
                new Category { Id = 2, Name = "Pantalones" },
                new Category { Id = 3, Name = "Remeras" },
                new Category { Id = 4, Name = "Camisas" },
                new Category { Id = 5, Name = "Busos" },
                new Category { Id = 6, Name = "Gorras" },
            };
            return categories;
        }
    }
}