using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Services;

public sealed class NotificationService : INotificationService
{
    private readonly IValidatorFactory _validatorFactory;
    private readonly Func<INotificationsRepository> _notificationsRepositoryFactory;
    private readonly IFireAndForgetService _fireAndForgetService;

    public NotificationService(IValidatorFactory validatorFactory, Func<INotificationsRepository> notificationsRepositoryFactory, IFireAndForgetService fireAndForgetService)
    {
        _validatorFactory = validatorFactory;
        _notificationsRepositoryFactory = notificationsRepositoryFactory;
        _fireAndForgetService = fireAndForgetService;
    }

    public async ValueTask<Guid> Post<T>(T notification, CancellationToken cancellationToken) where T : INotification
    {
        await _validatorFactory.GetValidator<T>().ValidateAndThrowAsync(notification, cancellationToken);

        INotificationsRepository repo = _notificationsRepositoryFactory();

        Guid notificationId = await repo.Create(notification, cancellationToken);

        _fireAndForgetService.PostTask(async (scope, cancellationToken) =>
        {
            var sender = scope.GetRequiredService<INotificationSender<T>>();

            await sender.Send(notification, cancellationToken);

            var repo = scope.GetRequiredService<INotificationsRepository>();

            // No token so that when the application begins its shutdown, the marking-as-done would be processed completely.
            await repo.MarkAsDelivered(notificationId, CancellationToken.None);
        });

        return notificationId;
    }

    public ValueTask<NotificationStatus> GetStatus(Guid notificationId, CancellationToken cancellationToken)
    {
        INotificationsRepository repo = _notificationsRepositoryFactory();

        return repo.GetStatus(notificationId, cancellationToken);
    }
}