namespace Municorn.TestTasks.Notifier.BusinessLogic.Config;

public sealed record ThrottleConfig(int Value, TimeSpan MinimumSendTime, TimeSpan MaximumSendTime);
