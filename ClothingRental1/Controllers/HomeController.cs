using BusinessLayer.Concrate;
using DataAccessLayer.Context;
using Microsoft.AspNetCore.Mvc;
using EntityLayer.Entities;
using X.PagedList;
public class HomeController : Controller
{
    private readonly ClothingRental1DbContext _context;
    private readonly ProductRepository _productRepository;
    private readonly CategoryRepository _categoryRepository;


    public HomeController(ClothingRental1DbContext context)
    {
        _context = context;
        _productRepository = new ProductRepository(_context);
        _categoryRepository=new CategoryRepository(_context);
    }


    public IActionResult Index(int? categoryId, int? page)
    {
        var categories = _categoryRepository.GetList(); // Kategori listesini alýn
        ViewData["Categories"] = categories; // ViewData'ya kategori listesini ekleyin

        var products = _productRepository.GetProductList();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId.Value).ToList();
        }

        var pagedList = products.ToPagedList(page ?? 1, 6);
        ViewBag.CurrentCategory = categoryId;

        return View(pagedList);
    }


}
