using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Contexts;


public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}