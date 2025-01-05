using DataAccessLayer.Context;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrate
{
    public class CartService
    {
        private readonly ClothingRental1DbContext _context;

        public CartService(ClothingRental1DbContext context)
        {
            _context = context;
        }

        public List<Cart> GetCartItems()
        {
            return _context.Carts
                           .Include(c => c.Product)
                           .ToList();
        }
    }
}
