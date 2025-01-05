using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using X.PagedList;
namespace ClothingRental1.Controllers
{
    // CategoryController.cs
    public class CategoryController : Controller
    {
        private readonly CategoryRepository _categoryRepository;

        public CategoryController(CategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
       
        public IActionResult CategoryList()
        {
            List<Category> categories = _categoryRepository.GetList();// Kategorilerinizi burada alın
            return PartialView(categories);
        }


        public ActionResult Details(int id)
        {
            var products = _categoryRepository.CategoryDetails(id);
            var pageNumber = 1; // Varsayılan sayfa numarası
            var pageSize = 10; // Sayfa başına düşen ürün sayısı
            var onePageOfProducts = new StaticPagedList<Product>(products, pageNumber, pageSize, products.Count);

            // Kategori listesi için ayrı bir model oluşturun
            List<Category> categories = _categoryRepository.GetList();

            // Ürün listesi ve kategori listesi için ayrı ayrı modelleri view'a geçirin
            ViewBag.Categories = categories;
            return View(onePageOfProducts);
        }
    }

}
