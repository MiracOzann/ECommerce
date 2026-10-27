namespace ECommerce.Catalog.Application;

public sealed record CreateCategoryCommand(string Name);

public sealed class CreateCategoryHandler(ICatalogDbContext db)
{
    public async Task<CategoryDto> HandleAsync(CreateCategoryCommand cmd, CancellationToken ct)
    {
        var name = cmd.Name.Trim();
        if (await db.Categories.AnyAsync(c => c.Name == name, ct))
            throw new ConflictException($"Bu kategori zaten var: {name}");

        var category = Category.Create(name);
        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
        return new CategoryDto(category.Id, category.Name);
    }
}

public sealed class ListCategoriesHandler(ICatalogDbContext db)
{
    public async Task<IReadOnlyList<CategoryDto>> HandleAsync(CancellationToken ct) =>
        await db.Categories.AsNoTracking().OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name)).ToListAsync(ct);
}
