using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Products;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Obtiene todos los productos disponibles.</summary>
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll([FromQuery] bool includeInactive = false)
    {
        var products = await _productService.GetAllProductsAsync(includeInactive);
        return Ok(products);
    }

    /// <summary>Obtiene un producto por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    /// <summary>Obtiene productos por categoría.</summary>
    [HttpGet("by-category/{categoryId:int}")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetByCategory(int categoryId)
    {
        var products = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(products);
    }

    /// <summary>Crea un nuevo producto.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
    {
        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Actualiza un producto.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponseDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var product = await _productService.UpdateProductAsync(id, dto);
        return Ok(product);
    }

    /// <summary>Elimina (desactiva) un producto.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteProductAsync(id);
        return NoContent();
    }

    /// <summary>Activa o desactiva la disponibilidad de un producto.</summary>
    [HttpPatch("{id:int}/availability")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponseDto>> ToggleAvailability(int id, [FromBody] ToggleAvailabilityDto dto)
    {
        var product = await _productService.ToggleAvailabilityAsync(id, dto.IsAvailable);
        return Ok(product);
    }
}

public record ToggleAvailabilityDto(bool IsAvailable);
