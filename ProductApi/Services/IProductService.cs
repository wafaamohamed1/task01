using ProductApi.Dto;

namespace ProductApi.Services;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto, CancellationToken ct);

    Task<IEnumerable<ProductResponseDto>> GetAllAsync(CancellationToken ct);

    Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken ct);

    Task<bool> UpdateAsync(int id, ProductUpdateDto dto, CancellationToken ct);

    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
