namespace Municorn.TestTasks.Notifier.BusinessLogic.Contracts;

public interface IFireAndForgetService
{
    void PostTask(Func<IServiceProvider, CancellationToken, ValueTask> task);
}
