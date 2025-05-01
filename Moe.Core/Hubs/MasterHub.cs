using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Moe.Core.Data;

namespace Moe.Core.Hubs;

[Authorize]
public class MasterHub : Hub
{
    private readonly MasterDbContext _context;

    public MasterHub(MasterDbContext context)
    {
        _context = context;
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
}