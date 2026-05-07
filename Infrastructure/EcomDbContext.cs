using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class EcomDbContext : DbContext
    {
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        public EcomDbContext(DbContextOptions<EcomDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Gaming Laptop",
                    Description = "High performance laptop",
                    Price = 4500.00,
                    Qty = 10
                },
                new Product
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Wireless Mouse",
                    Description = "Ergonomic mouse",
                    Price = 150.00,
                    Qty = 50
                }
            );
        }
    }

}
