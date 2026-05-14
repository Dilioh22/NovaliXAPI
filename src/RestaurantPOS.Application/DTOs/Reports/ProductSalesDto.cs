namespace RestaurantPOS.Application.DTOs.Reports;

public class ProductSalesDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public decimal AveragePrice { get; set; }
}
