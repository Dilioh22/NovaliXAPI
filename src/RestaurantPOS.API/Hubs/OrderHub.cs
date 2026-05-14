using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RestaurantPOS.API.Hubs;

[Authorize]
public class OrderHub : Hub
{
    private readonly ILogger<OrderHub> _logger;

    private static readonly HashSet<string> AllowedGroups = new(StringComparer.OrdinalIgnoreCase)
    {
        "kitchen", "cashier", "waiter", "tables"
    };

    public OrderHub(ILogger<OrderHub> logger)
    {
        _logger = logger;
    }

    public async Task JoinGroup(string groupName)
    {
        if (!AllowedGroups.Contains(groupName))
        {
            _logger.LogWarning("Connection {ConnectionId} attempted to join invalid group '{Group}'", Context.ConnectionId, groupName);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connection {ConnectionId} joined group {Group}", Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Connection {ConnectionId} left group {Group}", Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("LeftGroup", groupName);
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
