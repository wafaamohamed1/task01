using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Dto;

namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context, ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken ct)
    {
        try
        {
            var entity = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _context.Products.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Product created {ProductId} {Name} {Price}", entity.Id, entity.Name, entity.Price);

            return new ProductResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Price = entity.Price,
                CreatedAtUtc = entity.CreatedAtUtc
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product {Name} {Price}", dto.Name, dto.Price);
            throw;
        }
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken ct)
    {
        // AsNoTracking because this is a read-only operation and we don't need change tracking.
        return await _context.Products
            .AsNoTracking()
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .ToListAsync(ct);
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CreatedAtUtc = p.CreatedAtUtc
            })
            .SingleOrDefaultAsync(ct);

        if (product == null)
        {
            _logger.LogWarning("Product not found {ProductId}", id);
        }

        return product;
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct)
    {
        try
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null)
            {
                _logger.LogWarning("Update failed; product not found {ProductId}", id);
                return false;
            }

            existing.Name = dto.Name;
            existing.Price = dto.Price;

            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Product updated {ProductId} {Name} {Price}", existing.Id, existing.Name, existing.Price);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update product {ProductId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        try
        {
            var existing = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (existing == null)
            {
                _logger.LogWarning("Delete failed; product not found {ProductId}", id);
                return false;
            }

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Product deleted {ProductId} {Name}", existing.Id, existing.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete product {ProductId}", id);
            throw;
        }
    }
}
