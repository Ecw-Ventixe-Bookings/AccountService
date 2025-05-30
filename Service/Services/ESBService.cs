using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Service.Interfaces;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service.Services;

public class ESBService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<ESBService> _logger;

    private readonly ServiceBusClient _client;
    private readonly ServiceBusProcessor _processor;

    public ESBService(IServiceScopeFactory scopeFactory, IConfiguration config, ILogger<ESBService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;

        _client = new ServiceBusClient(_config["ESB:Connection"]);
        _processor = _client.CreateProcessor("createaccount", new ServiceBusProcessorOptions());
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += MessageHandler;
        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

       

        using (var scope = _scopeFactory.CreateScope())
        {
            var accountService = scope.ServiceProvider.GetService<IAccountService>();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var dto = JsonSerializer.Deserialize<AccountDto>(body, options);
            var result = await accountService!.CreateAccountAsync(dto!);

            _logger.LogInformation("\n--- START OF AZURE SERVICE BUS MESSAGE ---");
            _logger.LogInformation($"Body Reveived: {body}");
            _logger.LogInformation($"Result: {JsonSerializer.Serialize(result)}");
            _logger.LogInformation("\n--- END OF AZURE SERVICE BUS MESSAGE ---");
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogWarning("ESB message failed to deliver");
        return Task.CompletedTask;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _processor.StopProcessingAsync(cancellationToken);
        await _processor.DisposeAsync();
        await _client.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
