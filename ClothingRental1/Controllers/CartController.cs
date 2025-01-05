using DataAccessLayer.Context;
using Microsoft.AspNetCore.Mvc;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Newtonsoft.Json;

namespace ClothingRental1.Controllers
{
    public class CartController : Controller
    {
        private readonly ClothingRental1DbContext _context;

        public CartController(ClothingRental1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index(decimal? Tutar)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var username = User.Identity.Name;
                var kullanici = _context.User.FirstOrDefault(x => x.Email == username);
                var cartItems = _context.Carts.Include(c => c.Product).Where(x => x.UserId == kullanici.Id).ToList();

                if (cartItems.Any())
                {
                    Tutar = cartItems.Sum(x => x.Product.Price * x.Quantity);
                    ViewBag.Tutar = "Toplam Tutar = " + Tutar + " TL";

                    // Eğer rezervasyon bilgileri TempData üzerinde saklanmışsa, kaydet
                    if (TempData["Reservation"] != null)
                    {
                        var reservation = TempData["Reservation"] as Reservation;
                        _context.Reservations.Add(reservation);
                        _context.SaveChanges();

                        // Satış kaydı oluşturun
                        var sale = new Sales
                        {
                            ProductId = reservation.ProductId,
                            UserId = reservation.UserId,
                            Date = DateTime.Now,
                            Price = reservation.Products.Price
                        };
                        _context.Sales.Add(sale);
                        _context.SaveChanges();

                        // TempData'ı temizleyin
                        TempData.Remove("Reservation");
                    }

                    return View(cartItems);
                }
                else
                {
                    ViewBag.Tutar = "Sepetinizde ürün bulunmamaktadır";
                    return View(new List<Cart>()); // Boş bir liste döndür
                }
            }
            return NotFound();
        }


        public IActionResult AddCart(int id)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);
                var product = _context.Products.Find(id);

                if (model != null && product != null)
                {
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

                    // Rezervasyon nesnesini oluştur
                    var reservation = new Reservation
                    {
                        UserId = model.Id,
                        ProductId = product.Id,
                        StartDate = DateTime.Now, // Burada uygun tarihleri eklemelisin
                        EndDate = DateTime.Now.AddDays(1) // Örnek olarak bir sonraki günü ekledim, gerçek tarihleri buraya eklemelisin
                    };
                    _context.Reservations.Add(reservation);
                    _context.SaveChanges();

                    // Rezervasyon nesnesini geçici depolama alanında sakla
                    //TempData["Reservation"] = JsonConvert.SerializeObject(reservation);

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Login", "Account");
        }



        public IActionResult TotalCount(int? count)
        {
            if (User.Identity!.IsAuthenticated)
            {
                var model = _context.User.FirstOrDefault(x => x.Email == User.Identity.Name);
                count = _context.Carts.Count(x => x.UserId == model.Id);
                ViewBag.Count = count > 0 ? count.ToString() : "";
                return PartialView();
            }
            return NotFound();
        }

        public IActionResult Arttir(int id)
        {
            var cartItem = _context.Carts.Include(c => c.Product).FirstOrDefault(x => x.Id == id);
            if (cartItem != null)
            {
                if (cartItem.Product.Stock > cartItem.Quantity) // Stok miktarı kontrol ediliyor
                {
                    cartItem.Quantity++;
                    cartItem.Price = cartItem.Product.Price * cartItem.Quantity;
                    _context.SaveChanges();
                }
                else
                {
                    // Eğer stok miktarı yetersizse, kullanıcıya bilgi vermek için uygun bir mesaj döndürebilirsiniz.
                    ViewBag.ErrorMessage = "Üzgünüz, stokta yeterli ürün bulunmamaktadır.";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Azalt(int id)
        {
            var cartItem = _context.Carts.Include(c => c.Product).FirstOrDefault(x => x.Id == id);
            if (cartItem != null)
            {
                if (cartItem.Quantity > 1) // En az 1 ürün olmalı
                {
                    cartItem.Quantity--;
                    cartItem.Price = cartItem.Product.Price * cartItem.Quantity;
                    _context.SaveChanges();
                }
                else
                {
                    // Eğer miktar zaten 1 ise, ürünü tamamen kaldırmak için Delete eylemine yönlendirin.
                    return RedirectToAction("Delete", new { id = id });
                }
            }
            return RedirectToAction("Index");
        }

        public void DinamikMiktar(int id, int miktari)
        {
            var cartItem = _context.Carts.Include(c => c.Product).FirstOrDefault(x => x.Id == id);
            if (cartItem != null)
            {
                cartItem.Quantity = miktari;
                cartItem.Price = cartItem.Product.Price * cartItem.Quantity;
                _context.SaveChanges();
            }
        }

        public IActionResult Delete(int id)
        {
            var cartItem = _context.Carts.Find(id);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteRange()
        {
            if (User.Identity!.IsAuthenticated)
            {
                var kullaniciadi = User.Identity.Name;
                var model = _context.User.FirstOrDefault(x => x.Email == kullaniciadi);
                var cartItems = _context.Carts.Where(x => x.UserId == model.Id);
                _context.Carts.RemoveRange(cartItems);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return NotFound();
        }
    }
}
