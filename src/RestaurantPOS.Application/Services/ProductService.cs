using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Products;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class ProductService : IProductService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ProductResponseDto>> GetAllProductsAsync(bool includeInactive = false)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive && p.IsAvailable);

        var products = await query.OrderBy(p => p.Category.DisplayOrder).ThenBy(p => p.DisplayOrder).ToListAsync();
        return _mapper.Map<List<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new NotFoundException(nameof(Product), id);
        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        var category = await _context.Categories.FindAsync(dto.CategoryId)
            ?? throw new NotFoundException(nameof(Category), dto.CategoryId);

        var product = _mapper.Map<Product>(dto);
        product.Modifiers = dto.Modifiers.Select(m => new ProductModifier
        {
            Name = m.Name,
            PriceAdjustment = m.PriceAdjustment
        }).ToList();

        await _context.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        _ = await _context.Categories.FindAsync(dto.CategoryId)
            ?? throw new NotFoundException(nameof(Category), dto.CategoryId);

        _mapper.Map(dto, product);
        product.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();
        return _mapper.Map<List<ProductResponseDto>>(products);
    }

    public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Products.Where(p => p.IsActive))
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> GetCategoryByIdAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products.Where(p => p.IsActive))
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException(nameof(Category), id);
        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var category = _mapper.Map<Category>(dto);
        await _context.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);

        _mapper.Map(dto, category);
        category.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new NotFoundException(nameof(Category), id);

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ProductResponseDto> ToggleAvailabilityAsync(int id, bool isAvailable)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new NotFoundException(nameof(Product), id);

        product.IsAvailable = isAvailable;
        product.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return await GetProductByIdAsync(product.Id);
    }
}
