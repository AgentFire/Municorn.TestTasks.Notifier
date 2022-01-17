namespace Municorn.TestTasks.Notifier.BusinessLogic.Models;

public sealed record AndroidNotification(string DeviceToken, string Message, string Title, string? Condition = null) : NotificationBase(DeviceToken, Message);