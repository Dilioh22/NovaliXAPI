using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.CashRegister;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface ICashRegisterService
{
    Task<List<CashRegisterResponseDto>> GetAllCashRegistersAsync();
    Task<CashSessionResponseDto> OpenSessionAsync(OpenSessionDto dto);
    Task<CashSessionResponseDto> OpenSessionByNameAsync(string cashRegisterName, decimal openingAmount);
    Task<CashSessionResponseDto> CloseSessionAsync(int sessionId, CloseSessionDto dto);
    Task<CashSessionResponseDto?> GetActiveSessionAsync(int cashRegisterId);
    Task<CashSessionResponseDto?> GetMyActiveSessionAsync();
    Task<List<CashSessionResponseDto>> GetSessionHistoryAsync(int cashRegisterId, int take = 10);
    Task<CashSessionResponseDto> GetSessionByIdAsync(int sessionId);
}
