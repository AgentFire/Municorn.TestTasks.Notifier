using FluentValidation;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;

namespace Municorn.TestTasks.Notifier.BusinessLogic.Validators;

public sealed class AndroidNotificationValidator : NotificationValidatorBase<AndroidNotification>
{
    public AndroidNotificationValidator()
    {
        RuleFor(T => T.Title)
            .NotNull()
            .MaximumLength(255);

        RuleFor(T => T.Condition)
            .MaximumLength(200);
    }
}
