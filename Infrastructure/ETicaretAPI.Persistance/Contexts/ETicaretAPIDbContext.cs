﻿using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Common;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistance.Contexts
{
    public class ETicaretAPIDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public ETicaretAPIDbContext(DbContextOptions options) : base(options)
        {}

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<ProductImageFile> ProductImageFiles { get; set; }
        public DbSet<InvoiceFile> InvoiceFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>().HasKey(b => b.Id);

            builder.Entity<Basket>().HasOne(b=>b.Order).WithOne(b=>b.Basket).HasForeignKey<Order>(b=>b.Id);

            base.OnModelCreating(builder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //Change Tracker:Entityler üzerinde yapılan değişikliklerin yada yeni eklenen verinin yakalanmasını sağlayan propertydir
            //Update operasyonlarında Track edilen verileri yakalayıp elde etmemizi sağlar
            var datas=ChangeTracker.Entries<BaseEntity>();

            foreach (var data in datas)
            {
                if (data.State==EntityState.Added) {
                data.Entity.CreatedDate = DateTime.UtcNow;
                }
                else if (data.State==EntityState.Modified)
                {
                    data.Entity.UpdatedDate= DateTime.UtcNow;
                }

            } 
            return base.SaveChangesAsync(cancellationToken);

        }
    }
}
