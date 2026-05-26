using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseWindowsService();
builder.Host.UseSystemd();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext());

var port = builder.Configuration.GetValue<int>("ServiceOptions:Port", 5000);
var serviceName = builder.Configuration.GetValue<string>("ServiceOptions:Name") ?? "MySelfHostService";

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new
{
    ok = true,
    service = serviceName,
    time = DateTimeOffset.Now
}));

app.MapPost("/api/echo", (EchoRequest request, ILogger<Program> logger) =>
{
    logger.LogInformation("Received echo request from {from}", request.From);

    return Results.Ok(new
    {
        received = request,
        serverTime = DateTimeOffset.Now
    });
});

app.Run();

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started at {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }

        _logger.LogInformation("Worker stopped at {time}", DateTimeOffset.Now);
    }
}

public sealed record EchoRequest(string Message, string From);
