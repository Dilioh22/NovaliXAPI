using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Products;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllProductsAsync(bool includeInactive = false);
    Task<ProductResponseDto> GetProductByIdAsync(int id);
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto dto);
    Task DeleteProductAsync(int id);
    Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
    Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task DeleteCategoryAsync(int id);
    Task<ProductResponseDto> ToggleAvailabilityAsync(int id, bool isAvailable);
}
