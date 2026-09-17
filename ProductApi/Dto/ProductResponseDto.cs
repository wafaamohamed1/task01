using System;

namespace ProductApi.Dto;

public class ProductResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
