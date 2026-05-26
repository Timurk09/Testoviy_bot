using Microsoft.EntityFrameworkCore;
using Tst_bot.Models;

namespace Tst_bot.Database;

public class AppDbcontext : DbContext
{
    public AppDbcontext(DbContextOptions<AppDbcontext> options)
        : base(options){}
    public DbSet<User> Users { get; set; }
    public DbSet<Product>  Products { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Iphone X",
                Description = "Крутой телефон, который вы обязаны купить",
                Price = 30000,
                IsAvailable = true,
            },
            new Product
            {
                Id = 2,
                Name = "Airpods x",
                Description = "Лучшие наушники с наисочнейшим звуком",
                Price = 10000,
                IsAvailable = true,
            },
            new Product
            {
                Id = 3,
                Name = "Macbook X",
                Description = "Ноутбук, который потянет все ваши игрушки",
                Price = 200000,
                IsAvailable = true,
            },
            new Product
            {
                Id = 4,
                Name = "Энергос",
                Description = "Купи, если тебе срочно нужна энергия",
                Price = 200,
                IsAvailable = true
            });
    }
}