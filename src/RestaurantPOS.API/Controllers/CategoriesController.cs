using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Products;

namespace RestaurantPOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IProductService _productService;

    public CategoriesController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Obtiene todas las categorías activas.</summary>
    [HttpGet]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAll()
    {
        var categories = await _productService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>Obtiene una categoría por ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
    {
        var category = await _productService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    /// <summary>Crea una nueva categoría.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _productService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    /// <summary>Actualiza una categoría.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponseDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var category = await _productService.UpdateCategoryAsync(id, dto);
        return Ok(category);
    }

    /// <summary>Elimina (desactiva) una categoría.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteCategoryAsync(id);
        return NoContent();
    }
}
