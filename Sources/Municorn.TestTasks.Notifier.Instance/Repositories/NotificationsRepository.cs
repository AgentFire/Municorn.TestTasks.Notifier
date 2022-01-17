using Microsoft.EntityFrameworkCore;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using Municorn.TestTasks.Notifier.Data;
using Municorn.TestTasks.Notifier.Data.Models;

namespace Municorn.TestTasks.Notifier.Instance.Repositories;

public sealed class NotificationsRepository : INotificationsRepository
{
    private readonly DataContext _dataContext;

    public NotificationsRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async ValueTask<Guid> Create<T>(T notification, CancellationToken cancellationToken) where T : INotification
    {
        var model = new Notification
        {
            Status = NotificationStatus.NotDelivered,
            Token = notification.Token,
            MessageText = notification.MessageText
        };

        _dataContext.Notifications.Add(model);

        await _dataContext.SaveChangesAsync(cancellationToken);

        return model.Id;
    }

    public async ValueTask MarkAsDelivered(Guid notificationId, CancellationToken cancellationToken)
    {
        Notification notification = await _dataContext.Notifications.Where(T => T.Id == notificationId).SingleAsync(cancellationToken);

        notification.Status = NotificationStatus.Delivered;

        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<NotificationStatus> GetStatus(Guid notificationId, CancellationToken cancellationToken)
    {
        return await _dataContext.Notifications.Where(T => T.Id == notificationId).Select(T => T.Status).SingleAsync(cancellationToken);
    }
}
