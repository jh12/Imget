using Microsoft.AspNetCore.SignalR;

namespace Imget.Hubs;

public class TagHub : Hub<ITagHub>
{
    public async Task CreateTag(string name, string description)
    {
        await Clients.All.UpdateTag(name, description);
    }
}

public interface ITagHub
{
    Task UpdateTag(string name, string description);
}