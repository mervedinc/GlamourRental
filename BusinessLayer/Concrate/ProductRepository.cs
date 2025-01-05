using BusinessLayer.Abstract;
using DataAccessLayer.Context;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Concrate
{
    public class ProductRepository : GenericRepository<Product>
    {
        private readonly ClothingRental1DbContext _context;

        public ProductRepository(ClothingRental1DbContext context) : base(context)
        {
            _context = context;
        }

        public Product GetById(int id)
        {
            return _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
        }


        public List<Product> GetProductList()
        {
            return _context.Products.Include(p => p.Category).ToList();
        }
    }
}
