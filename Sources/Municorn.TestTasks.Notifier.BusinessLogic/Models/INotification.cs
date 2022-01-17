namespace Municorn.TestTasks.Notifier.BusinessLogic.Models;

public interface INotification
{
    string Token { get; }
    string MessageText { get; }
}
