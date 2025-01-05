using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingRental1.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ClothingRental1DbContext _context;

        public ReservationsController(ClothingRental1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var user = _context.User.FirstOrDefault(x => x.Email == username);

                var cartItems = _context.Carts
                    .Include(c => c.Product)
                    .Where(c => c.UserId == user.Id)
                    .ToList();

                if (cartItems.Any())
                {
                    // Rezervasyonlar oluşturulacak
                    foreach (var cartItem in cartItems)
                    {
                        var reservation = new Reservation
                        {
                            StartDate = DateTime.Now, // Bu kısmı gereksinimlerinize göre güncelleyin
                            EndDate = DateTime.Now.AddDays(7), // Örnek olarak 7 gün sonrasını alıyoruz
                            ProductId = cartItem.ProductId,
                            UserId = user.Id
                        };

                        _context.Reservations.Add(reservation);
                    }

                    _context.SaveChanges(); // Rezervasyonları kaydet

                    // Sepeti temizle
                    _context.Carts.RemoveRange(cartItems);
                    _context.SaveChanges();

                    ViewBag.Message = "Rezervasyonlar başarıyla oluşturuldu ve sepet temizlendi.";
                }
                else
                {
                    ViewBag.Message = "Sepetinizde ürün bulunmamaktadır.";
                }
            }
            else
            {
                ViewBag.Message = "Kullanıcı oturumu bulunamadı.";
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int productId, Reservation reservation)
        {
            if (User.Identity.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);
                if (model == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    return NotFound();
                }

                reservation.Products = product;
                reservation.ProductId = productId;
                reservation.UserId = model.Id;

                var overlappingReservations = _context.Reservations
                    .Where(r => r.ProductId == productId &&
                                ((reservation.StartDate >= r.StartDate && reservation.StartDate <= r.EndDate) ||
                                 (reservation.EndDate >= r.StartDate && reservation.EndDate <= r.EndDate)))
                    .ToList();

                // Bu tarihlerde kaç rezervasyon olduğunu kontrol et
                var overlappingCount = overlappingReservations.Count();

                // Ürünün stok miktarını kontrol et
                if (overlappingCount >= product.Stock)
                {
                    ModelState.AddModelError("", "Seçilen tarihlerde ürünün stoğu yetersiz.");
                    ViewBag.Product = product;
                    ViewBag.UserId = model.Id;
                    return View(reservation);
                }
                var newCartItem = new Cart
                {
                    UserId = model.Id,
                    ProductId = product.Id,
                    Quantity = 1,
                    Price = product.Price,
                    Date = DateTime.Now
                };
                _context.Carts.Add(newCartItem);
                _context.SaveChanges();
                TempData["ReservationStartDate"] = reservation.StartDate;
                TempData["ReservationEndDate"] = reservation.EndDate;

                

                // Sepeti göstermek ve satın almak için yönlendirme yapın
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction("Login", "Account");
        }


        public IActionResult Create(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
            {
                return NotFound();
            }

            var kullaniciadi = User.Identity.Name;
            var user = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);

            ViewBag.ProductId = productId;  // Set the ProductId instead of the entire Product object
            ViewBag.UserId = user?.Id ?? 0;
            return View();
        }

        public IActionResult Details(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Products)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }
   
        public JsonResult GetUnavailableDates(int productId)
        {
            var unavailableDates = _context.Reservations
                .Where(r => r.ProductId == productId)
                .Select(r => new { r.StartDate, r.EndDate })
                .ToList();

            return Json(unavailableDates);
        }
    }
}
