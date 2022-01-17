using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Municorn.TestTasks.Notifier.Data.Models;

public sealed class Notification
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    public string Token { get; init; }
    public string MessageText { get; init; }

    public NotificationStatus Status { get; set; }
}