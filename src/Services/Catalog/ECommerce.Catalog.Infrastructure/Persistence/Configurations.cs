namespace ECommerce.Catalog.Infrastructure.Persistence;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("categories");
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).ValueGeneratedNever();
        b.Property(c => c.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(c => c.Name).IsUnique();
    }
}

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");
        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();
        b.Property(p => p.Name).HasMaxLength(200).IsRequired();
        b.Property(p => p.Description).HasMaxLength(2000);
        b.Property(p => p.Sku).HasMaxLength(64).IsRequired();
        b.Property(p => p.Price).HasPrecision(18, 2);
        b.HasIndex(p => p.Sku).IsUnique();
        b.HasIndex(p => p.CategoryId);
        b.HasOne(p => p.Category).WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
