using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Contracts;

public interface INotificationsRepository
{
    ValueTask<Guid> Create<T>(T notification, CancellationToken cancellationToken) where T : INotification;
    ValueTask MarkAsDelivered(Guid notificationId, CancellationToken cancellationToken);
    ValueTask<NotificationStatus> GetStatus(Guid notificationId, CancellationToken cancellationToken);
}
