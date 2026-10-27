namespace ECommerce.Catalog.Application;

// Commands
public sealed record CreateProductCommand(string Name, string? Description, string Sku, decimal Price, Guid CategoryId);
public sealed record UpdateProductCommand(Guid Id, string Name, string? Description, decimal Price, Guid CategoryId);

public sealed class CreateProductHandler(ICatalogDbContext db)
{
    public async Task<ProductDto> HandleAsync(CreateProductCommand cmd, CancellationToken ct)
    {
        var category = await db.Categories.FindAsync([cmd.CategoryId], ct)
            ?? throw new NotFoundException($"Kategori bulunamadı: {cmd.CategoryId}");
        
        var sku = cmd.Sku.Trim().ToUpperInvariant();
        if (await db.Products.AnyAsync(p => p.Sku == sku, ct))
            throw new ConflictException($"Bu SKU zaten kayıtlı: {sku}");

        var product = Product.Create(cmd.Name, cmd.Description, sku, cmd.Price, cmd.CategoryId);
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);

        return ToDto(product, category.Name);

    }
    internal static ProductDto ToDto(Product p, string categoryName) =>
        new(p.Id, p.Name, p.Description, p.Sku, p.Price, p.CategoryId, categoryName, p.IsActive);

}

public sealed class UpdateProductHandler(ICatalogDbContext db)
{
    public async Task<ProductDto> HandleAsync(UpdateProductCommand cmd, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == cmd.Id && p.IsActive, ct)
            ?? throw new NotFoundException($"Ürün bulunamadı: {cmd.Id}");
        
        var category = await db.Categories.FindAsync([cmd.CategoryId], ct)
            ?? throw new NotFoundException($"Kategori bulunamadı: {cmd.CategoryId}");

        product.Update(cmd.Name, cmd.Description, cmd.Price, cmd.CategoryId);
        await db.SaveChangesAsync(ct);
        return CreateProductHandler.ToDto(product, category.Name);
    }
}

public sealed class DeleteProductHandler(ICatalogDbContext db)
{
    public async Task HandleAsync(Guid id, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive, ct)
            ?? throw new NotFoundException($"Ürün bulunamadı: {id}");
        product.Deactivate();
        await db.SaveChangesAsync(ct);
    }
}

// ---------- Queries ----------
public sealed record ListProductsQuery(int Page = 1, int PageSize = 20, Guid? CategoryId = null, string? Search = null);

public sealed class GetProductHandler(ICatalogDbContext db)
{
    public async Task<ProductDto> HandleAsync(Guid id, CancellationToken ct) =>
        await db.Products.AsNoTracking()
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Sku, p.Price,
                p.CategoryId, p.Category.Name, p.IsActive))
            .FirstOrDefaultAsync(ct)
        ?? throw new NotFoundException($"Ürün bulunamadı: {id}");
}

public sealed class ListProductsHandler(ICatalogDbContext db)
{
    public async Task<PagedResult<ProductDto>> HandleAsync(ListProductsQuery query, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var q = db.Products.AsNoTracking().Where(p => p.IsActive);
        if (query.CategoryId is { } categoryId)
            q = q.Where(p => p.CategoryId == categoryId);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(p => p.Name.ToLower().Contains(term));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Sku, p.Price,
                p.CategoryId, p.Category.Name, p.IsActive))
            .ToListAsync(ct);

        return new PagedResult<ProductDto>(items, page, pageSize, total);
    }
}