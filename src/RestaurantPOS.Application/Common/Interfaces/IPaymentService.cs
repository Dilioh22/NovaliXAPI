using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Payments;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto);
    Task<List<PaymentResponseDto>> GetPaymentsByOrderAsync(int orderId);
    Task<List<PaymentMethodDto>> GetPaymentMethodsAsync();
}

public class PaymentMethodDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
