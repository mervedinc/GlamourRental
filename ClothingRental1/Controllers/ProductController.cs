using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ClothingRental1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository productRepository;

        public ProductController(ClothingRental1DbContext context)
        {
            productRepository = new ProductRepository(context);
        }

        public IActionResult Details(int id)
        {
            var product = productRepository.GetById(id);
            if (product == null)
            {
                // Ürün bulunamadıysa uygun bir hata mesajı veya sayfa göster
                return NotFound();
            }
            var pagedList = new StaticPagedList<Product>(new[] { product }, 1, 1, 1);
            return View(pagedList);
        }
    }


}

