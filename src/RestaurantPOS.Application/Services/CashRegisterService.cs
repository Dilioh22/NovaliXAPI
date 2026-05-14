using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Application.Common.Exceptions;
using RestaurantPOS.Application.Common.Interfaces;
using RestaurantPOS.Application.DTOs.CashRegister;
using RestaurantPOS.Domain.Entities;
using RestaurantPOS.Domain.Interfaces;

namespace RestaurantPOS.Application.Services;

public class CashRegisterService : ICashRegisterService
{
    private readonly IAppDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public CashRegisterService(IAppDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<CashRegisterResponseDto>> GetAllCashRegistersAsync()
    {
        var registers = await _context.CashRegisters
            .Include(cr => cr.Sessions.Where(s => s.Status == "Open"))
                .ThenInclude(s => s.OpenedBy)
            .Where(cr => cr.IsActive)
            .ToListAsync();

        return registers.Select(cr =>
        {
            var dto = _mapper.Map<CashRegisterResponseDto>(cr);
            dto.CurrentSession = cr.Sessions.FirstOrDefault() is { } session
                ? _mapper.Map<CashSessionResponseDto>(session)
                : null;
            return dto;
        }).ToList();
    }

    public async Task<CashSessionResponseDto> OpenSessionAsync(OpenSessionDto dto)
    {
        var register = await _context.CashRegisters.FindAsync(dto.CashRegisterId)
            ?? throw new NotFoundException(nameof(CashRegister), dto.CashRegisterId);

        var existing = await _context.CashRegisterSessions
            .AnyAsync(s => s.CashRegisterId == dto.CashRegisterId && s.Status == "Open");
        if (existing)
            throw new BusinessRuleException("Esta caja ya tiene una sesión abierta.");

        var userId = _currentUser.UserId
            ?? throw new BusinessRuleException("Usuario no autenticado.");

        var session = new CashRegisterSession
        {
            CashRegisterId = dto.CashRegisterId,
            OpenedById = userId,
            OpeningAmount = dto.OpeningAmount,
            Notes = dto.Notes,
            Status = "Open",
            OpenedAt = DateTime.UtcNow
        };

        await _context.CashRegisterSessions.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<CashSessionResponseDto> CloseSessionAsync(int sessionId, CloseSessionDto dto)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.Payments).ThenInclude(p => p.PaymentMethod)
            .Include(s => s.CashRegister)
            .Include(s => s.OpenedBy)
            .FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new NotFoundException(nameof(CashRegisterSession), sessionId);

        if (session.Status != "Open")
            throw new BusinessRuleException("Esta sesión ya fue cerrada.");

        var userId = _currentUser.UserId
            ?? throw new BusinessRuleException("Usuario no autenticado.");

        var totalCash = session.Payments.Where(p => p.PaymentMethod.Code == "CASH").Sum(p => p.Amount);
        var totalCard = session.Payments.Where(p => p.PaymentMethod.Code == "CARD").Sum(p => p.Amount);
        var totalTransfer = session.Payments.Where(p => p.PaymentMethod.Code == "TRANSFER").Sum(p => p.Amount);
        var totalSales = session.Payments.Sum(p => p.Amount);

        session.ClosedById = userId;
        session.ClosingAmount = dto.ClosingAmount;
        session.TotalCash = totalCash;
        session.TotalCard = totalCard;
        session.TotalTransfer = totalTransfer;
        session.TotalSales = totalSales;
        session.TransactionCount = session.Payments.Count;
        session.ExpectedAmount = session.OpeningAmount + totalCash;
        session.Difference = dto.ClosingAmount - session.ExpectedAmount;
        session.Status = "Closed";
        session.ClosedAt = DateTime.UtcNow;
        session.Notes = dto.Notes ?? session.Notes;
        session.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return await GetSessionByIdAsync(sessionId);
    }

    public async Task<CashSessionResponseDto> OpenSessionByNameAsync(string cashRegisterName, decimal openingAmount)
    {
        var register = await _context.CashRegisters
            .FirstOrDefaultAsync(cr => cr.Name == cashRegisterName && cr.IsActive);

        if (register == null)
        {
            register = new CashRegister
            {
                Name = cashRegisterName,
                IsActive = true
            };
            await _context.CashRegisters.AddAsync(register);
            await _unitOfWork.SaveChangesAsync();
        }

        var existing = await _context.CashRegisterSessions
            .AnyAsync(s => s.CashRegisterId == register.Id && s.Status == "Open");
        if (existing)
            throw new BusinessRuleException("Esta caja ya tiene una sesión abierta.");

        var userId = _currentUser.UserId
            ?? throw new BusinessRuleException("Usuario no autenticado.");

        var session = new CashRegisterSession
        {
            CashRegisterId = register.Id,
            OpenedById = userId,
            OpeningAmount = openingAmount,
            Status = "Open",
            OpenedAt = DateTime.UtcNow
        };

        await _context.CashRegisterSessions.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        return await GetSessionByIdAsync(session.Id);
    }

    public async Task<CashSessionResponseDto?> GetMyActiveSessionAsync()
    {
        var userId = _currentUser.UserId;
        if (userId == null) return null;

        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.OpenedBy)
            .Include(s => s.ClosedBy)
            .FirstOrDefaultAsync(s => s.OpenedById == userId && s.Status == "Open");

        return session == null ? null : _mapper.Map<CashSessionResponseDto>(session);
    }

    public async Task<CashSessionResponseDto?> GetActiveSessionAsync(int cashRegisterId)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.OpenedBy)
            .Include(s => s.ClosedBy)
            .FirstOrDefaultAsync(s => s.CashRegisterId == cashRegisterId && s.Status == "Open");

        return session == null ? null : _mapper.Map<CashSessionResponseDto>(session);
    }

    public async Task<List<CashSessionResponseDto>> GetSessionHistoryAsync(int cashRegisterId, int take = 10)
    {
        var sessions = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.OpenedBy)
            .Include(s => s.ClosedBy)
            .Where(s => s.CashRegisterId == cashRegisterId)
            .OrderByDescending(s => s.OpenedAt)
            .Take(take)
            .ToListAsync();
        return _mapper.Map<List<CashSessionResponseDto>>(sessions);
    }

    public async Task<CashSessionResponseDto> GetSessionByIdAsync(int sessionId)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Include(s => s.OpenedBy)
            .Include(s => s.ClosedBy)
            .FirstOrDefaultAsync(s => s.Id == sessionId)
            ?? throw new NotFoundException(nameof(CashRegisterSession), sessionId);
        return _mapper.Map<CashSessionResponseDto>(session);
    }
}
