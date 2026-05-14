using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Reservations;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReservationService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<ReservationResponseDto>> GetAllAsync(DateTime? date = null, string? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Table).ThenInclude(t => t.Zone)
            .Where(r => r.IsActive);

        if (date.HasValue)
            query = query.Where(r => r.ReservationDate.Date == date.Value.Date);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        var list = await query.OrderBy(r => r.ReservationDate).ToListAsync();
        return _mapper.Map<List<ReservationResponseDto>>(list);
    }

    public async Task<List<ReservationResponseDto>> GetByTableAsync(int tableId)
    {
        var list = await _context.Reservations
            .Include(r => r.Table).ThenInclude(t => t.Zone)
            .Where(r => r.TableId == tableId && r.IsActive)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();
        return _mapper.Map<List<ReservationResponseDto>>(list);
    }

    public async Task<ReservationResponseDto> GetByIdAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Table).ThenInclude(t => t.Zone)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive)
            ?? throw new NotFoundException(nameof(Reservation), id);

        return _mapper.Map<ReservationResponseDto>(reservation);
    }

    public async Task<ReservationResponseDto> CreateAsync(CreateReservationDto dto)
    {
        var table = await _context.Tables
            .Include(t => t.Zone)
            .FirstOrDefaultAsync(t => t.Id == dto.TableId && t.IsActive)
            ?? throw new NotFoundException(nameof(Table), dto.TableId);

        var reservation = _mapper.Map<Reservation>(dto);
        await _context.Reservations.AddAsync(reservation);

        // Marcar la mesa como reservada si está libre
        if (table.Status == "Libre")
        {
            table.Status = "Reservada";
            table.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(reservation.Id);
    }

    public async Task<ReservationResponseDto> UpdateAsync(int id, UpdateReservationDto dto)
    {
        var reservation = await _context.Reservations
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive)
            ?? throw new NotFoundException(nameof(Reservation), id);

        _ = await _context.Tables.FindAsync(dto.TableId)
            ?? throw new NotFoundException(nameof(Table), dto.TableId);

        reservation.TableId = dto.TableId;
        reservation.CustomerName = dto.CustomerName;
        reservation.CustomerPhone = dto.CustomerPhone;
        reservation.PartySize = dto.PartySize;
        reservation.ReservationDate = dto.ReservationDate;
        reservation.Notes = dto.Notes;
        reservation.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(reservation.Id);
    }

    public async Task<ReservationResponseDto> UpdateStatusAsync(int id, UpdateReservationStatusDto dto)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive)
            ?? throw new NotFoundException(nameof(Reservation), id);

        reservation.Status = dto.Status;
        reservation.UpdatedAt = DateTime.UtcNow;

        // Si se cancela o completa, liberar la mesa si sigue reservada por esta reserva
        if ((dto.Status == "Cancelada" || dto.Status == "Completada" || dto.Status == "NoShow")
            && reservation.Table.Status == "Reservada")
        {
            reservation.Table.Status = "Libre";
            reservation.Table.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        return await GetByIdAsync(reservation.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive)
            ?? throw new NotFoundException(nameof(Reservation), id);

        reservation.IsActive = false;
        reservation.UpdatedAt = DateTime.UtcNow;

        // Liberar la mesa si sigue reservada
        if (reservation.Status == "Pendiente" || reservation.Status == "Confirmada")
        {
            if (reservation.Table.Status == "Reservada")
            {
                reservation.Table.Status = "Libre";
                reservation.Table.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
