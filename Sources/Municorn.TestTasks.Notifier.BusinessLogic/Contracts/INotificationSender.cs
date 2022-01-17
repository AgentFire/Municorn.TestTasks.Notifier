using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Contracts;

public interface INotificationSender<T> where T : INotification
{
    ValueTask Send(T notification, CancellationToken cancellationToken);
}
