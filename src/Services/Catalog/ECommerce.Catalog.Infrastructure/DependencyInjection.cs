namespace ECommerce.Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddCatalogInfrastructure(this IHostApplicationBuilder builder)
    {
        // "catalogdb" AppHost'taki veritabanı kaynağının adıyla aynı olmalı
        builder.AddNpgsqlDbContext<CatalogDbContext>("catalogdb");
        builder.Services.AddScoped<ICatalogDbContext>(sp => sp.GetRequiredService<CatalogDbContext>());
        return builder;
    }

    public static async Task MigrateCatalogDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.Database.MigrateAsync();
    }
}