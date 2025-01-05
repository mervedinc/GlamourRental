
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using EntityLayer.Entities;
namespace DataAccessLayer.Context
{
    public class ClothingRental1DbContext : DbContext
    {
        public ClothingRental1DbContext(DbContextOptions<ClothingRental1DbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Reservation> Reservations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany(p => p.Carts)
                .HasForeignKey(c => c.ProductId);

            base.OnModelCreating(modelBuilder);
        }
    }
}