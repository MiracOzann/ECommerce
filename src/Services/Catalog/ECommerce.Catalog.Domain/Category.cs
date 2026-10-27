namespace ECommerce.Catalog.Domain;

public sealed class Category
{
    private Category() { }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;

    public static Category Create(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Category { Id = Guid.CreateVersion7(), Name = name.Trim() };
    }
}
