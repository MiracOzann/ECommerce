namespace ECommerce.Catalog.Api;

public sealed record CreateProductRequest(
    [Required, StringLength(200)] string Name,
    [StringLength(2000)] string? Description,
    [Required, StringLength(64)] string Sku,
    [Range(0.01, 1_000_000)] decimal Price,
    Guid CategoryId);

public sealed record UpdateProductRequest(
    [Required, StringLength(200)] string Name,
    [StringLength(2000)] string? Description,
    [Range(0.01, 1_000_000)] decimal Price,
    Guid CategoryId);

public sealed record CreateCategoryRequest([Required, StringLength(100)] string Name);

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var products = app.MapGroup("/api/products").WithTags("Products");

        products.MapGet("/", async ([AsParameters] ListProductsQuery query,
            ListProductsHandler h, CancellationToken ct) =>
            Results.Ok(await h.HandleAsync(query, ct)));

        products.MapGet("/{id:guid}", async (Guid id, GetProductHandler h, CancellationToken ct) =>
            Results.Ok(await h.HandleAsync(id, ct)));

        products.MapPost("/", async (CreateProductRequest r, CreateProductHandler h, CancellationToken ct) =>
        {
            var dto = await h.HandleAsync(new(r.Name, r.Description, r.Sku, r.Price, r.CategoryId), ct);
            return Results.Created($"/api/products/{dto.Id}", dto);
        });

        products.MapPut("/{id:guid}", async (Guid id, UpdateProductRequest r, UpdateProductHandler h, CancellationToken ct) =>
            Results.Ok(await h.HandleAsync(new(id, r.Name, r.Description, r.Price, r.CategoryId), ct)));

        products.MapDelete("/{id:guid}", async (Guid id, DeleteProductHandler h, CancellationToken ct) =>
        {
            await h.HandleAsync(id, ct);
            return Results.NoContent();
        });

        var categories = app.MapGroup("/api/categories").WithTags("Categories");

        categories.MapGet("/", async (ListCategoriesHandler h, CancellationToken ct) =>
            Results.Ok(await h.HandleAsync(ct)));

        categories.MapPost("/", async (CreateCategoryRequest r, CreateCategoryHandler h, CancellationToken ct) =>
        {
            var dto = await h.HandleAsync(new(r.Name), ct);
            return Results.Created($"/api/categories/{dto.Id}", dto);
        });

        return app;
    }
}