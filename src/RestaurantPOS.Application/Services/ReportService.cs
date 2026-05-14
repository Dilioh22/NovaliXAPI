using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.Reports;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class ReportService : IReportService
{
    private readonly IAppDbContext _context;

    public ReportService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<SalesReportDto> GetSalesReportAsync(DateTime from, DateTime to)
    {
        var toEnd = to.Date.AddDays(1).AddTicks(-1);
        var orders = await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Category)
            .Include(o => o.Payments).ThenInclude(p => p.PaymentMethod)
            .Include(o => o.Waiter)
            .Where(o => o.Status == "Pagado" && o.CreatedAt >= from && o.CreatedAt <= toEnd)
            .ToListAsync();

        var totalSales = orders.Sum(o => o.Total);
        var totalTax = orders.Sum(o => o.TaxAmount);
        var totalDiscount = orders.Sum(o => o.DiscountAmount);
        var totalTips = orders.Sum(o => o.TipAmount);

        var salesByDay = orders
            .GroupBy(o => o.CreatedAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SalesByDayDto
            {
                Date = g.Key,
                Total = g.Sum(o => o.Total),
                OrderCount = g.Count()
            }).ToList();

        var salesByPaymentMethod = orders
            .SelectMany(o => o.Payments)
            .GroupBy(p => p.PaymentMethod.Name)
            .Select(g => new SalesByPaymentMethodDto
            {
                Method = g.Key,
                Total = g.Sum(p => p.Amount),
                Count = g.Count()
            }).ToList();

        var waiterPerformance = orders
            .GroupBy(o => new { o.WaiterId, WaiterName = o.Waiter.FullName })
            .Select(g => new WaiterPerformanceDto
            {
                WaiterId = g.Key.WaiterId,
                WaiterName = g.Key.WaiterName,
                OrderCount = g.Count(),
                TotalSales = g.Sum(o => o.Total),
                AverageTicket = g.Count() > 0 ? g.Sum(o => o.Total) / g.Count() : 0,
                TotalTips = g.Sum(o => o.TipAmount)
            }).ToList();

        var topProducts = await GetTopProductsAsync(from, to, 10);

        return new SalesReportDto
        {
            From = from,
            To = to,
            TotalSales = totalSales,
            TotalTax = totalTax,
            TotalDiscount = totalDiscount,
            TotalTips = totalTips,
            NetSales = totalSales - totalTax,
            TotalOrders = orders.Count,
            AverageTicket = orders.Count > 0 ? totalSales / orders.Count : 0,
            SalesByDay = salesByDay,
            SalesByPaymentMethod = salesByPaymentMethod,
            WaiterPerformance = waiterPerformance,
            TopProducts = topProducts
        };
    }

    public async Task<List<ProductSalesDto>> GetTopProductsAsync(DateTime from, DateTime to, int top = 10)
    {
        var toEnd = to.Date.AddDays(1).AddTicks(-1);
        var items = await _context.OrderItems
            .Include(i => i.Product).ThenInclude(p => p.Category)
            .Include(i => i.Order)
            .Where(i => i.Order.Status == "Pagado"
                && i.Order.CreatedAt >= from
                && i.Order.CreatedAt <= toEnd)
            .ToListAsync();

        return items
            .GroupBy(i => new { i.ProductId, i.Product.Name, CategoryName = i.Product.Category?.Name ?? "" })
            .Select(g => new ProductSalesDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                CategoryName = g.Key.CategoryName,
                Quantity = g.Sum(i => i.Quantity),
                Total = g.Sum(i => i.Subtotal),
                AveragePrice = g.Average(i => i.UnitPrice)
            })
            .OrderByDescending(p => p.Quantity)
            .Take(top)
            .ToList();
    }

    public async Task<SalesReportDto> GetDailySalesReportAsync(DateTime date)
    {
        return await GetSalesReportAsync(date.Date, date.Date);
    }
}
