namespace Municorn.TestTasks.Notifier.BusinessLogic.Models;

public sealed record IosNotification(string PushToken, string Alert, int Priority, bool IsBackground) : NotificationBase(PushToken, Alert);