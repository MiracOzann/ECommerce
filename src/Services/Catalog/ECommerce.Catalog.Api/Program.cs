var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddCatalogInfrastructure();
builder.Services.AddApplication();

builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapDefaultEndpoints();
app.MapCatalogEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.MigrateCatalogDatabaseAsync(); // Sadece geliştirmede otomatik migration
}

app.Run();