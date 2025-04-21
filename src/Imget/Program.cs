using Autofac;
using Autofac.Extensions.DependencyInjection;
using Imget.Components;
using Imget.Endpoints;
using Imget.Hubs;
using Imget.Module;
using Imget.Shared.Bus;
using Imget.Shared.Message.Event;
using Microsoft.AspNetCore.ResponseCompression;
using Scalar.AspNetCore;
using MudBlazor.Services;
using Rebus.Bus;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder
    .Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(b => b.RegisterModule<MainModule>());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var services = builder.Services;

services.AddSerilog();
services.AddOpenApi();

services.AddSignalR();
services.AddResponseCompression(o =>
{
    o.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]);
});

services.AddMudServices();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    app.MapOpenApi();
    app.MapScalarApiReference("/scalar/{documentName}", o =>
    {
        o.WithDefaultHttpClient(ScalarTarget.Shell, ScalarClient.Curl)
            .WithTheme(ScalarTheme.DeepSpace)
            .AddServer("localhost:6001");

        o.EnabledClients = [ScalarClient.Curl, ScalarClient.WebRequest, ScalarClient.HttpClient];
    });
}
else
{
    app.UseResponseCompression();
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Imget.Client._Imports).Assembly);

app.MapScalarEndpoints()
    .MapImageEndpoints();

app.MapHub<TagHub>("/taghub");
app.MapHub<ImageHub>("/imagehub");

IBus bus = app.Services.GetRequiredService<IBus>();
await bus.SubscribeInboundImageEvents();

app.Run();