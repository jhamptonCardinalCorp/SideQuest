
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text;
using System.IO;

public class Worker : BackgroundService
{
    private readonly IHostApplicationLifetime _appLifetime;

    public Worker(IHostApplicationLifetime appLifetime)
    {
        _appLifetime = appLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var builder = WebApplication.CreateBuilder();
        var app = builder.Build();

        const string validator = "FMIG123456";
        const string logFilePath = "meraki_payloads.txt";

        app.MapGet("/api/meraki", async context =>
        {
            await context.Response.WriteAsync(validator);
        });

        app.MapPost("/api/meraki", async context =>
        {
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            // Log payload to file
            await File.AppendAllTextAsync(logFilePath, $"{DateTime.UtcNow}: {body}{Environment.NewLine}");

            // Optional: parse JSON and log device count
            try
            {
                var jsonDoc = JsonDocument.Parse(body);
                if (jsonDoc.RootElement.TryGetProperty("observations", out var observations))
                {
                    var count = observations.GetArrayLength();
                    await File.AppendAllTextAsync(logFilePath, $"Device count: {count}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                await File.AppendAllTextAsync(logFilePath, $"Error parsing JSON: {ex.Message}{Environment.NewLine}");
            }

            context.Response.StatusCode = 200;
        });

        await app.RunAsync(stoppingToken);
    }
}
