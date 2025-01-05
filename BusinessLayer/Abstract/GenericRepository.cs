using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Abstract
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        ClothingRental1DbContext db;
        DbSet<T> data;

       

        public GenericRepository(ClothingRental1DbContext context)
        {
            db = context;
            data = db.Set<T>();
        }


        public void Delete(T p)
        {
            var entry = db.Entry(p);
            if (entry.State == EntityState.Detached)
            {
                data.Attach(p);
            }
            data.Remove(p);
            db.SaveChanges();
        }

        public T GetById(int id)
        {
            return data.Find(id);

        }

        public void Insert(T p)
        {
            data.Add(p);
            db.SaveChanges();
        }

        public List<T> List()
        {
            return data.ToList();
        }

        public void Update(T p)
        {
            //db.Entry<T>(p).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
