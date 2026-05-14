namespace RestaurantPOS.Application.Common.Interfaces;

public interface IOrderNumberService
{
    Task<string> GenerateAsync();
}
