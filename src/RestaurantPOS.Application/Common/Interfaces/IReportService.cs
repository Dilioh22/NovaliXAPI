using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Application.DTOs.Reports;

namespace RestaurantPOS.Application.Common.Interfaces;

public interface IReportService
{
    Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to);
    Task<List<ProductSalesDto>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10);
    Task<SalesReportDto> GetDailySalesReportAsync(DateTime date);
}
