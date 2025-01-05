using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClothingRental1.Controllers
{

    public class AdminCategoryController : Controller
    {
        private readonly CategoryRepository categoryRepository;

        public AdminCategoryController(ClothingRental1DbContext context)
        {
            categoryRepository = new CategoryRepository(context);
        }

        public ActionResult Index()
        {
            var categories = categoryRepository.List();
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]

        public ActionResult Create(Category p)
        {
            if (ModelState.IsValid)
            {
                categoryRepository.Insert(p);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Bİr hata oluştu");
            return View();
        }

        public ActionResult Delete(int id)
        {
            var category = categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            categoryRepository.Delete(category);
            return RedirectToAction("Index");
        }

        public ActionResult Update(int id)
        {
            var category = categoryRepository.GetById(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Category model)
        {
            if (ModelState.IsValid)
            {
                // Güncellenmek istenen kategori veritabanında var mı diye kontrol edilir
                var existingCategory = categoryRepository.GetById(model.Id);
                if (existingCategory == null)
                {
                    return NotFound();
                }

                // Var olan kategorinin alanları güncellenir
                existingCategory.Name = model.Name;
                existingCategory.Description = model.Description;

                // Güncelleme işlemi yapılır
                categoryRepository.Update(existingCategory);

                return RedirectToAction("Index");
            }
            // Eğer ModelState.IsValid false dönerse, hata mesajları ile birlikte güncelleme sayfası tekrar gösterilir
            return View(model);
        }

    }
}
