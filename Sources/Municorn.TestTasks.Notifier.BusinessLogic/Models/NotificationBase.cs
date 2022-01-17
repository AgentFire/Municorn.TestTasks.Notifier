namespace Municorn.TestTasks.Notifier.BusinessLogic.Models;

public abstract record NotificationBase : INotification
{
    private readonly string _token;
    private readonly string _messageText;

    string INotification.Token => _token;
    string INotification.MessageText => _messageText;

    public NotificationBase(string token, string messageText)
    {
        _token = token;
        _messageText = messageText;
    }
}
