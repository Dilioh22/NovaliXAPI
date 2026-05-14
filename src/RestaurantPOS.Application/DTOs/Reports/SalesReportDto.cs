using System;
using System.Collections.Generic;

namespace RestaurantPOS.Application.DTOs.Reports;

public class SalesReportDto
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTips { get; set; }
    public decimal NetSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageTicket { get; set; }
    public List<SalesByDayDto> SalesByDay { get; set; } = new();
    public List<SalesByPaymentMethodDto> SalesByPaymentMethod { get; set; } = new();
    public List<ProductSalesDto> TopProducts { get; set; } = new();
    public List<WaiterPerformanceDto> WaiterPerformance { get; set; } = new();
}

public class SalesByDayDto
{
    public DateTime Date { get; set; }
    public decimal Total { get; set; }
    public int OrderCount { get; set; }
}

public class SalesByPaymentMethodDto
{
    public string Method { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}

public class WaiterPerformanceDto
{
    public int WaiterId { get; set; }
    public string WaiterName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
    public decimal AverageTicket { get; set; }
    public decimal TotalTips { get; set; }
}
