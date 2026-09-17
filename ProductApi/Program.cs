using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Dto;
using ProductApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure DbContext using connection string from configuration (user-secrets in development)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Use user-secrets or environment variables.");
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// ProductService should be scoped so it can consume a scoped AppDbContext per request.
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

app.MapPost("/products", async ([FromBody] ProductCreateDto dto, IProductService service, CancellationToken ct) =>
{
    if (dto == null)
        return Results.BadRequest();

    var created = await service.CreateAsync(dto, ct);

    var location = $"/products/{created.Id}";
    return Results.Created(location, created);
});

app.MapGet("/products", async (IProductService service, CancellationToken ct) =>
{
    var items = await service.GetAllAsync(ct);
    return Results.Ok(items);
});

app.MapGet("/products/{id:int}", async (int id, IProductService service, CancellationToken ct) =>
{
    var item = await service.GetByIdAsync(id, ct);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

app.MapPut("/products/{id:int}", async (int id, [FromBody] ProductUpdateDto dto, IProductService service, CancellationToken ct) =>
{
    if (dto == null)
        return Results.BadRequest();

    var updated = await service.UpdateAsync(id, dto, ct);
    return updated ? Results.NoContent() : Results.NotFound();
});

app.MapDelete("/products/{id:int}", async (int id, IProductService service, CancellationToken ct) =>
{
    var deleted = await service.DeleteAsync(id, ct);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
