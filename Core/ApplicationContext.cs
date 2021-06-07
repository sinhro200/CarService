using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ApplicationContext : DbContext
    {
        private string _connStr;

        public ApplicationContext(string connectionString)
        {
            _connStr = connectionString;
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(_connStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder
            //    .Entity<Brand>()
            //    .HasMany(b => b.Models)
            //    .WithOne(m => m.Brand)
            //    .HasForeignKey(m => m.BrandId);

            modelBuilder
                .Entity<Model>()
                .HasOne(m => m.Brand)
                .WithMany(b => b.Models)    
                .HasForeignKey(m => m.BrandId);

            modelBuilder.Entity<Model>()
                .Navigation(m => m.Brand)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder
                .Entity<Order>()
                .HasMany(c => c.Services)
                .WithMany(s => s.Orders)
                .UsingEntity<OrderService> (
                   j => j
                    .HasOne(pt => pt.Service)
                    .WithMany(t => t.OrderServices)
                    .HasForeignKey(pt => pt.ServiceId),
                j => j
                    .HasOne(pt => pt.Order)
                    .WithMany(p => p.OrderServices)
                    .HasForeignKey(os => os.OrderId),
                j => {
                    //j.Property(pt => pt.UserServices).HasDefaultValueSql("CURRENT_TIMESTAMP");
                    //j.Property(pt => pt.Mark).HasDefaultValue(3);
                    j.HasKey(t => new { t.OrderId, t.ServiceId});
                    j.ToTable("OrderService");
                }
            );

            modelBuilder
                .Entity<Order>()
                .Property(o => o.DateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder
                .Entity<User>()
                .HasMany<Car>(c => c.Cars)
                .WithOne(c => c.Owner)
                .IsRequired()
                .HasForeignKey(c => c.OwnerId)
                .IsRequired();

            modelBuilder
                .Entity<Car>()
                .HasOne(c => c.Model)
                .WithMany(m => m.Cars);

            modelBuilder
                .Entity<Model>()
                .HasMany(m => m.Cars)
                .WithOne(c => c.Model);
        }
    }
}
