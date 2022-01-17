namespace Municorn.TestTasks.Notifier.Instance.Web.Models.In;

public sealed record AndroidNotificationDto(string DeviceToken, string Message, string Title, string? Condition = null);