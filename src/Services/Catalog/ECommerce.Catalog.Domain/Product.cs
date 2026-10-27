namespace ECommerce.Catalog.Domain;

public sealed class Product
{
    private Product() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string Sku { get; private set; } = default!;
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Product Create(string name, string? description, string sku, decimal price, Guid categoryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        var product = new Product
        {
            Id = Guid.CreateVersion7(),
            Sku = sku.Trim().ToUpperInvariant(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        product.Apply(name, description, price, categoryId);
        return product;
    }

    public void Update(string name, string? description, decimal price, Guid categoryId)
    {
        Apply(name, description, price, categoryId);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private void Apply(string name, string? description, decimal price, Guid categoryId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);
        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        CategoryId = categoryId;
    }
}