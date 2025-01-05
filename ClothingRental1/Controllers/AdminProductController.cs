using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using EntityLayer.Entities;
//using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace ClothingRental1.Controllers
{
    public class AdminProductController(ClothingRental1DbContext context) : Controller
    {

        ProductRepository productRepository = new ProductRepository(context);
        private readonly ClothingRental1DbContext _context = context;



        public IActionResult Index(int? page)
        {
            int pageSize = 6;
            int pageNumber = (page ?? 1);

            var products = _context.Products
                .Include(p => p.Category) // Kategori bilgisini dahil ediyoruz
                .OrderBy(p => p.Id)
                .ToPagedList(pageNumber, pageSize);

            ViewBag.OnePageOfProducts = products;

            return View(products.ToList());
        }



        public IActionResult Create()
        {
            List<Category> kategoriler = _context.Category.ToList();

            List<SelectListItem> kategoriListesi = kategoriler.Select(kategori => new SelectListItem
            {
                Text = kategori.Name,
                Value = kategori.Id.ToString()
            }).ToList();

            ViewBag.CategoryList = kategoriListesi;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Product data, IFormFile File)
        {
            if (User.Identity!.IsAuthenticated)
            {
                List<Category> kategoriler = _context.Category.ToList();
                List<SelectListItem> kategoriListesi = kategoriler.Select(kategori => new SelectListItem
                {
                    Text = kategori.Name,
                    Value = kategori.Id.ToString()
                }).ToList();

                ViewBag.CategoryList = kategoriListesi;

                ModelState.AddModelError("", "Hata oluştu");
                return View(data);
            }

            if (File != null && File.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                // Klasör var olup olmadığını kontrol et, yoksa oluştur
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    File.CopyTo(stream);
                }

                data.Image = "/images/" + uniqueFileName;
                productRepository.Insert(data);
            }
            else
            {
                List<Category> kategoriler = _context.Category.ToList();
                List<SelectListItem> kategoriListesi = kategoriler.Select(kategori => new SelectListItem
                {
                    Text = kategori.Name,
                    Value = kategori.Id.ToString()
                }).ToList();

                ViewBag.CategoryList = kategoriListesi;

                ModelState.AddModelError("", "Dosya yüklenemedi");
                return View(data);
            }

            return RedirectToAction("Index");
        }



        public IActionResult Delete(int id)
        {
            var delete = productRepository.GetById(id);
            if (delete == null)
            {
                return NotFound();
            }

            // İlgili rezervasyon kayıtlarını sil
            var reservations = _context.Reservations.Where(r => r.ProductId == id).ToList();
            if (reservations.Any())
            {
                _context.Reservations.RemoveRange(reservations);
                _context.SaveChanges();
            }

            // İlgili satış kayıtlarını sil
            var sales = _context.Sales.Where(s => s.ProductId == id).ToList();
            if (sales.Any())
            {
                _context.Sales.RemoveRange(sales);
                _context.SaveChanges();
            }

            // Ürünü sil
            productRepository.Delete(delete);
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            var update = productRepository.GetById(id);
            if (update == null)
            {
                return NotFound();
            }

            List<SelectListItem> categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

            ViewBag.ktgr = categories;

            return View(update);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Product data, IFormFile File)
        {
            var update = productRepository.GetById(data.Id);
            if (update == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (File != null && File.Length > 0)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // Klasör var olup olmadığını kontrol et, yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Eski resmi sil
                    string oldImagePath = Path.Combine(uploadsFolder, update.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        File.CopyTo(stream);
                    }

                    // Yeni dosya adını güncelle
                    update.Image = uniqueFileName;
                }

                update.Description = data.Description;
                update.Name = data.Name;
                update.IsApproved = data.IsApproved;
                update.Price = data.Price;
                update.Stock = data.Stock;
                update.CategoryId = data.CategoryId;

                productRepository.Update(update);

                return RedirectToAction("Index");
            }

            List<SelectListItem> categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

            ViewBag.ktgr = categories;

            return View(update);
        }
    }

 }


