using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Imget.Client.Pages;

public partial class Tag : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;

    private HubConnection? _hubConnection;

    private List<TagSlim> _tags { get; } = [new("Name", "Description")];

    public Tag(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    protected override async Task OnInitializedAsync()
    {
        Console.WriteLine("Init");

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/taghub"))
            .WithAutomaticReconnect()
            .WithStatefulReconnect()
            .Build();

        _hubConnection.On<string, string>("UpdateTag", (name, description) =>
        {
            int existingIndex = _tags.FindIndex(t => t.Name == name);

            if (existingIndex >= 0)
            {
                _tags[existingIndex] = new TagSlim(name, description);
            }
            else
            {
                _tags.Add(new TagSlim(name, description));
            }

            InvokeAsync(StateHasChanged);
        });

        await _hubConnection.StartAsync();
    }

    private async Task Create()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.SendAsync("CreateTag", $"Test_{Random.Shared.Next(0, 4)}", $"Desc {Random.Shared.Next(0, 10)}");
            Console.WriteLine("Test ");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }

    private record TagSlim(string Name, string Description);
}