using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Imget.Client.Pages;

public partial class Monitor : IAsyncDisposable
{
    private readonly NavigationManager _navigationManager;

    private HubConnection? _imageHub;

    private List<string> _imageUrls { get; } = [];

    public Monitor(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    protected override async Task OnInitializedAsync()
    {
        _imageHub = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri("/imagehub"))
            .WithAutomaticReconnect()
            .WithStatefulReconnect()
            .Build();

        _imageHub.On<string>("Discovered", (url) =>
        {
            _imageUrls.Add(url);

            InvokeAsync(StateHasChanged);
        });

        await _imageHub.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_imageHub is not null)
        {
            await _imageHub.DisposeAsync();
        }
    }
}