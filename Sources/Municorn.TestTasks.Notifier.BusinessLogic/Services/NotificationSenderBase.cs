using Microsoft.Extensions.Logging;
using Municorn.TestTasks.Notifier.BusinessLogic.Config;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Services;

public abstract class NotificationSenderBase<T> : INotificationSender<T> where T : INotification
{
    private int _counter = 0;

    protected ThrottleConfig Config { get; }
    protected ILogger Logger { get; }

    public NotificationSenderBase(ThrottleConfig config, ILogger logger)
    {
        Config = config;
        Logger = logger;
    }

    public async ValueTask Send(T notification, CancellationToken cancellationToken)
    {
        if (Interlocked.Increment(ref _counter) % Config.Value == 0)
        {
            Logger.LogInformation("Skipping fifth message");
            return;
        }

#pragma warning disable CA2254 // Bug: https://github.com/dotnet/roslyn-analyzers/issues/5626
        Logger.LogInformation($@"Sending notification ""{notification.MessageText}""");
#pragma warning restore CA2254

        await SendInternal(notification, cancellationToken);
    }

    protected abstract ValueTask SendInternal(T notification, CancellationToken cancellationToken);
}
