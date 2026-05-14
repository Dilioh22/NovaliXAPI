using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Tables;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface ITableService
{
    Task<List<TableZoneResponseDto>> GetAllZonesWithTablesAsync();
    Task<List<TableResponseDto>> GetAllTablesAsync();
    Task<TableResponseDto> GetTableByIdAsync(int id);
    Task<TableResponseDto> CreateTableAsync(CreateTableDto dto);
    Task<TableResponseDto> UpdateTableAsync(int id, UpdateTableDto dto);
    Task<TableResponseDto> UpdateTableStatusAsync(int id, UpdateTableStatusDto dto);
    Task DeleteTableAsync(int id);
    Task<TableZoneResponseDto> CreateZoneAsync(CreateTableZoneDto dto);
    Task<List<TableZoneResponseDto>> GetAllZonesAsync();
}
