using Microsoft.EntityFrameworkCore;

namespace ProductApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(b =>
        {
           // b.HasKey(p => p.Id);
            b.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Enforce non-negative price at the database level
            b.HasCheckConstraint("CK_Product_Price_NonNegative", "Price >= 0");

            // Record creation time (UTC) on the DB side if not provided
            b.Property(p => p.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
