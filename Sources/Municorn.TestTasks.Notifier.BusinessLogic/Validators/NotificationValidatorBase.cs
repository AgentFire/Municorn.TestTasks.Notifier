using FluentValidation;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Validators;

public abstract class NotificationValidatorBase<T> : AbstractValidator<T> where T : INotification
{
    protected NotificationValidatorBase()
    {
        RuleFor(T => T.Token)
            .NotNull()
            .MaximumLength(50);

        RuleFor(T => T.MessageText)
            .NotNull()
            .MaximumLength(2000);
    }
}
