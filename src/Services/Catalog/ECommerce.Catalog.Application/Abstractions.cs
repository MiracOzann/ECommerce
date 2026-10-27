namespace ECommerce.Catalog.Application;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public sealed class NotFoundException(string message) : Exception(message);
public sealed class ConflictException(string message) : Exception(message);

public sealed record ProductDto(Guid Id, string Name, string? Description, string Sku,
    decimal Price, Guid CategoryId, string CategoryName, bool IsActive);

public sealed record CategoryDto(Guid Id, string Name);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount);
