using Autofac;
using Microsoft.EntityFrameworkCore;
using Municorn.TestTasks.Notifier.Data;
using Municorn.TestTasks.Notifier.Instance.Configuration;

namespace Municorn.TestTasks.Notifier.Instance.Services;

internal sealed class StartupService : IHostedService
{
    private readonly ILifetimeScope _scope;
    private readonly FakesConfig _fakesConfig;
    private readonly ILogger<StartupService> _logger;

    public StartupService(ILifetimeScope scope, FakesConfig fakesConfig, ILogger<StartupService> logger)
    {
        _scope = scope;
        _fakesConfig = fakesConfig;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_fakesConfig.Database)
        {
            await MigrateDatabase();
            _logger.LogInformation("Database is up to date");
        }
        else
        {
            _logger.LogInformation("Database is being faked and needs no migration");
        }
        
        _logger.LogInformation("Default Url: http://localhost:8075\r\nSwagger Url: /swagger\r\nHealth Checks Url: /health");
    }

    private async Task MigrateDatabase()
    {
        await using ILifetimeScope scope = _scope.BeginLifetimeScope();

        DataContext db = scope.Resolve<DataContext>();
        db.Database.Migrate();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
