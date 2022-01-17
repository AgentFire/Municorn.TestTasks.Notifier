using Autofac;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using System.Diagnostics;

namespace Municorn.TestTasks.Notifier.Instance.Services;

public sealed class FireAndForgetService : BackgroundService, IFireAndForgetService
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ILifetimeScope _scope;
    private readonly ILogger<FireAndForgetService> _logger;
    private readonly HashSet<Task> _currentTasks = new();

    public FireAndForgetService(ILifetimeScope scope, ILogger<FireAndForgetService> logger)
    {
        _scope = scope;
        _logger = logger;
    }

    public void PostTask(Func<IServiceProvider, CancellationToken, ValueTask> task)
    {
        Task host = null!;

        lock (_currentTasks)
        {
            host = Task.Run(async () =>
            {
                await using ILifetimeScope scope = _scope.BeginLifetimeScope();

                try
                {
                    await task(scope.Resolve<IServiceProvider>(), _cts.Token);
                }
                catch (OperationCanceledException) when (_cts.Token.IsCancellationRequested) { /* Swallow */ }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Demystify(), "Unhandled exception on a forgotten task");
                    // Swallow exception.
                }
                finally
                {
                    lock (_currentTasks)
                    {
                        _currentTasks.Remove(host);
                    }
                }
            });

            _currentTasks.Add(host);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var source = new TaskCompletionSource();

        await using var _ = stoppingToken.Register(() =>
        {
            _cts.Cancel();
            source.SetResult();
        });

        await source.Task;

        // Wait for all forgotten tasks.
        while (_currentTasks.Count > 0)
        {
            await Task.Delay(100, CancellationToken.None);
        }
    }
}
