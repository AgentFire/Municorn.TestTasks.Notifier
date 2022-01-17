using Microsoft.Extensions.Logging;
using Municorn.TestTasks.Notifier.BusinessLogic.Config;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Services;

public sealed class AndroidNotificationSender : NotificationSenderBase<AndroidNotification>
{
    private static readonly Random _randomizer = new();

    public AndroidNotificationSender(ThrottleConfig config, ILogger<AndroidNotificationSender> logger) : base(config, logger) { }

    protected override async ValueTask SendInternal(AndroidNotification notification, CancellationToken cancellationToken)
    {
        TimeSpan selectedSendTime = TimeSpan.FromMilliseconds(_randomizer.Next((int)Config.MinimumSendTime.TotalMilliseconds, (int)Config.MaximumSendTime.TotalMilliseconds));

        await Task.Delay(selectedSendTime, cancellationToken);
        Logger.LogInformation("Message sent");
    }
}
