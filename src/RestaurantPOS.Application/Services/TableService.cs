using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Tables;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class TableService : ITableService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TableService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TableZoneResponseDto>> GetAllZonesWithTablesAsync()
    {
        var zones = await _context.TableZones
            .Include(z => z.Tables.Where(t => t.IsActive))
                .ThenInclude(t => t.Orders.Where(o => o.IsActive && o.Status != "Pagado" && o.Status != "Cancelado"))
            .Where(z => z.IsActive)
            .OrderBy(z => z.Name)
            .ToListAsync();
        return _mapper.Map<List<TableZoneResponseDto>>(zones);
    }

    public async Task<List<TableResponseDto>> GetAllTablesAsync()
    {
        var tables = await _context.Tables
            .Include(t => t.Zone)
            .Include(t => t.Orders.Where(o => o.IsActive && o.Status != "Pagado" && o.Status != "Cancelado"))
            .Where(t => t.IsActive)
            .OrderBy(t => t.Number)
            .ToListAsync();
        return _mapper.Map<List<TableResponseDto>>(tables);
    }

    public async Task<TableResponseDto> GetTableByIdAsync(int id)
    {
        var table = await _context.Tables
            .Include(t => t.Zone)
            .Include(t => t.Orders.Where(o => o.IsActive && o.Status != "Pagado" && o.Status != "Cancelado"))
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException(nameof(Table), id);
        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task<TableResponseDto> CreateTableAsync(CreateTableDto dto)
    {
        _ = await _context.TableZones.FindAsync(dto.ZoneId)
            ?? throw new NotFoundException(nameof(TableZone), dto.ZoneId);

        var existing = await _context.Tables
            .AnyAsync(t => t.ZoneId == dto.ZoneId && t.Number == dto.Number && t.IsActive);
        if (existing)
            throw new BusinessRuleException($"Ya existe una mesa con el número {dto.Number} en esa zona.");

        var table = _mapper.Map<Table>(dto);
        await _context.Tables.AddAsync(table);
        await _unitOfWork.SaveChangesAsync();
        return await GetTableByIdAsync(table.Id);
    }

    public async Task<TableResponseDto> UpdateTableAsync(int id, UpdateTableDto dto)
    {
        var table = await _context.Tables
            .Include(t => t.Zone)
            .FirstOrDefaultAsync(t => t.Id == id && t.IsActive)
            ?? throw new NotFoundException(nameof(Table), id);

        _ = await _context.TableZones.FindAsync(dto.ZoneId)
            ?? throw new NotFoundException(nameof(TableZone), dto.ZoneId);

        var duplicate = await _context.Tables
            .AnyAsync(t => t.ZoneId == dto.ZoneId && t.Number == dto.Number && t.IsActive && t.Id != id);
        if (duplicate)
            throw new BusinessRuleException($"Ya existe una mesa con el número {dto.Number} en esa zona.");

        table.ZoneId    = dto.ZoneId;
        table.Number    = dto.Number;
        table.Capacity  = dto.Capacity;
        table.PositionX = dto.PositionX;
        table.PositionY = dto.PositionY;
        table.Shape     = dto.Shape;
        table.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return await GetTableByIdAsync(table.Id);
    }

    public async Task<TableResponseDto> UpdateTableStatusAsync(int id, UpdateTableStatusDto dto)
    {
        var table = await _context.Tables
            .Include(t => t.Zone)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new NotFoundException(nameof(Table), id);

        table.Status = dto.Status;
        table.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TableResponseDto>(table);
    }

    public async Task DeleteTableAsync(int id)
    {
        var table = await _context.Tables.FindAsync(id)
            ?? throw new NotFoundException(nameof(Table), id);

        table.IsActive = false;
        table.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<TableZoneResponseDto> CreateZoneAsync(CreateTableZoneDto dto)
    {
        var zone = _mapper.Map<TableZone>(dto);
        await _context.TableZones.AddAsync(zone);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TableZoneResponseDto>(zone);
    }

    public async Task<List<TableZoneResponseDto>> GetAllZonesAsync()
    {
        var zones = await _context.TableZones
            .Where(z => z.IsActive)
            .OrderBy(z => z.Name)
            .ToListAsync();
        return _mapper.Map<List<TableZoneResponseDto>>(zones);
    }
}
