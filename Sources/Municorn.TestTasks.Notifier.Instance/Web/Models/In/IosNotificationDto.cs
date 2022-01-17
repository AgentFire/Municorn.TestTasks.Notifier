namespace Municorn.TestTasks.Notifier.Instance.Web.Models.In;

public sealed record IosNotificationDto(string PushToken, string Alert, int Priority = 10, bool IsBackground = true);
