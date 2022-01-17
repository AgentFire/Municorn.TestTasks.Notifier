using System.ComponentModel.DataAnnotations;

namespace Municorn.TestTasks.Notifier.Data.Models;

public sealed record NotificationRequest(
    [property: Key] Guid RequestId,
    Guid ResultNotificationId
);