using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using X.PagedList;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Concrate;


namespace ClothingRental1.Controllers
{
    public class SalesController : Controller
    {
        // GET: Sales
        private readonly ClothingRental1DbContext _context;

        public SalesController(ClothingRental1DbContext context)
        {
            _context = context;
        }


        public IActionResult Index(int sayfa = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                int pageSize = 6;
                var kullaniciadi = User.Identity.Name;
                var kullanici = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);

                if (kullanici == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var model = _context.Sales
                    .Include(s => s.Product)
                    .Where(s => s.UserId == kullanici.Id)
                    .OrderBy(p => p.Id)
                    .ToPagedList(sayfa, pageSize);

                return View(model);
            }

            return NotFound();
        }

        public IActionResult Buy(int id)
        {
            var model = _context.Carts.Include(c => c.Product).FirstOrDefault(x => x.Id == id);
            if (model == null || model.Product == null)
            {
                return NotFound();
            }
            return View(model);
        }


        [HttpPost]
        public IActionResult Buy2(int id)
        {
            try
            {
                var model = _context.Carts.FirstOrDefault(x => x.Id == id);
                if (model != null && ModelState.IsValid)
                {
                    var satis = new Sales
                    {
                        UserId = model.UserId,
                        ProductId = model.ProductId,
                        Quantity = model.Quantity,
                        Image = model.Image,
                        Price = model.Price,
                        Date = DateTime.Now,
                    };

                    _context.Carts.Remove(model);
                    _context.Sales.Add(satis);

                    // Rezervasyonu kaydet
                    var reservation = new Reservation
                    {
                        UserId = model.UserId,
                        ProductId = model.ProductId,
                        StartDate = TempData["ReservationStartDate"] != null ? (DateTime)TempData["ReservationStartDate"] : DateTime.MinValue,
                        EndDate = TempData["ReservationEndDate"] != null ? (DateTime)TempData["ReservationEndDate"] : DateTime.MinValue
                    };
                    _context.Reservations.Add(reservation);

                    _context.SaveChanges();

                    // Başarılı kiralama mesajını TempData'ya ekleyin
                    TempData["SuccessMessage"] = "Kiralama İşlemi Başarılı Bir Şekilde Gerçekleşmiştir";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kiralama İşlemi Başarısız: " + ex.Message;
            }

            // Cart/Index sayfasına yönlendirin
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult BuyAll2()
        {
            var kullaniciadi = User.Identity.Name;
            var kullanici = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);

            if (kullanici != null)
            {
                // Geçici tarihleri TempData üzerinden al
                var startDate = TempData["ReservationStartDate"] as DateTime?;
                var endDate = TempData["ReservationEndDate"] as DateTime?;

                if (startDate != null && endDate != null)
                {
                    var model = _context.Carts.Include(c => c.Product).Where(x => x.UserId == kullanici.Id).ToList();

                    if (model != null && model.Any())
                    {
                        foreach (var item in model)
                        {
                            var satis = new Sales
                            {
                                UserId = item.UserId,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                Price = item.Price,
                                Image = item.Product.Image,
                                Date = DateTime.Now
                            };

                            _context.Sales.Add(satis);
                        }

                        // Rezervasyon oluşturma ve kaydetme
                        var reservation = new Reservation
                        {
                            // Reservation özelliklerini ayarla (ürün, kullanıcı, tarihler...)
                            UserId = kullanici.Id,
                            ProductId = model.First().ProductId, // Örnek olarak ilk ürünün Id'sini alabilirsiniz, tüm sepetin aynı ürün olduğunu varsayıyorum
                            StartDate = startDate.Value,
                            EndDate = endDate.Value
                        };

                        _context.Reservations.Add(reservation);

                        // Sepeti boşaltma
                        _context.Carts.RemoveRange(model);

                        // Değişiklikleri kaydetme
                        _context.SaveChanges();

                        // Başarılı durumda işlem yap
                        TempData["SuccessMessage"] = "Kiralama İşlemi Başarılı Bir Şekilde Gerçekleşmiştir";
                        return RedirectToAction("Index", "Cart");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Sepetinizde ürün bulunmamaktadır";
                        return RedirectToAction("Index", "Cart");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Rezervasyon tarihleri eksik veya hatalı.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Index", "Cart");
            }
        }

        public IActionResult BuyAll(decimal? Tutar)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var kullanici = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);
                var model = _context.Carts.Where(x => x.UserId == kullanici.Id).ToList();
                var kid = _context.Carts.FirstOrDefault(x => x.UserId == kullanici.Id);
                if (model != null)
                {
                    if (kid == null)
                    {
                        ViewBag.Tutar = "Siparişiniz başarıyla oluşturuldu!";
                    }
                    else if (kid != null)
                    {
                        Tutar = _context.Carts.Where(x => x.UserId == kid.UserId).Sum(x => x.Product.Price * x.Quantity);
                        ViewBag.Tutar = "Toplam Tutar = " + Tutar + "TL";
                    }
                    return View(model);
                }

                return View();
            }
            return NotFound();
        }

    }
}
