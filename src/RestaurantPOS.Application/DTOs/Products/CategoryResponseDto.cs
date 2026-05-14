namespace RestaurantPOS.Application.DTOs.Products;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public string? IconName { get; set; }
}
