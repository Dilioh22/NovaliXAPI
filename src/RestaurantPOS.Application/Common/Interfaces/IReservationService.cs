using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Reservations;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IReservationService
{
    Task<List<ReservationResponseDto>> GetAllAsync(DateTime? date = null, string? status = null);
    Task<List<ReservationResponseDto>> GetByTableAsync(int tableId);
    Task<ReservationResponseDto> GetByIdAsync(int id);
    Task<ReservationResponseDto> CreateAsync(CreateReservationDto dto);
    Task<ReservationResponseDto> UpdateAsync(int id, UpdateReservationDto dto);
    Task<ReservationResponseDto> UpdateStatusAsync(int id, UpdateReservationStatusDto dto);
    Task DeleteAsync(int id);
}
