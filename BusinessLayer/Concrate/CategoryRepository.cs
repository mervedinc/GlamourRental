using BusinessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

public class CategoryRepository : GenericRepository<Category>
{
    private readonly ClothingRental1DbContext _context;

    public CategoryRepository(ClothingRental1DbContext context) : base(context)
    {
        _context = context;
    }
    public List<Category> GetList()
    {
        return _context.Category.Include(p => p.Products).ToList();
    }
    //Diğer metodlar buraya eklenecek...

    public List<Product> CategoryDetails(int id)
    {
        return _context.Products.Where(x => x.CategoryId == id).ToList();
    }

}
