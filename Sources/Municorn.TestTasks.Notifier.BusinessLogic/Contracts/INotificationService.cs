using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Contracts;

public interface INotificationService
{
    ValueTask<Guid> Post<T>(T notification, CancellationToken cancellationToken) where T : INotification;
    ValueTask<NotificationStatus> GetStatus(Guid notificationId, CancellationToken cancellationToken);
}
