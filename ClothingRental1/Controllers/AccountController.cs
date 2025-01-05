using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace ClothingRental1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ClothingRental1DbContext _context;

        public AccountController(ClothingRental1DbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
            {
                ViewBag.UserName = User.Identity.Name;
                // Diğer kullanıcıya özel verileri burada ayarlayabilirsiniz.
            }

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User data)
        {
            var user = _context.User.FirstOrDefault(x => x.Email == data.Email && x.Password == PasswordHelper.HashPassword(data.Password));
            if (user != null)
            {
                // Authenticate user
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    // Add additional claims as needed
                };

                var identity = new ClaimsIdentity(claims, "cookie");
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Set session variables
                HttpContext.Session.SetString("Mail", user.Email);
                HttpContext.Session.SetString("Ad", user.Name);
                HttpContext.Session.SetString("Soyad", user.SurName);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Hata = "Kullanıcı Adı Veya Şifreniz Yanlış";
            return View(data);
        }

        [HttpPost]
        public IActionResult Register(User data)
        {
            data.Password = PasswordHelper.HashPassword(data.Password);
            data.RePassword = PasswordHelper.HashPassword(data.RePassword);

            _context.User.Add(data);
            data.Role = "User";

            _context.SaveChanges();

            // Authenticate user after registration
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, data.Email),
                // Add additional claims as needed
            };

            var identity = new ClaimsIdentity(claims, "cookie");
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            ViewBag.register = "Kayıt işlemi başarılı, giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        // PasswordHelper sınıfı
        public class PasswordHelper
        {
            public static string HashPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // Şifreyi byte dizisine dönüştür
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    // Hash'i hex formata dönüştür
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
        }
    }
}
