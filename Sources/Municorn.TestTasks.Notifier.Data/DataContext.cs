using Microsoft.EntityFrameworkCore;
using Municorn.TestTasks.Notifier.Data.Models;

namespace Municorn.TestTasks.Notifier.Data;

public sealed class DataContext : DbContext
{
    public DbSet<NotificationRequest> NotificationRequests { get; init; }
    public DbSet<Notification> Notifications { get; init; }

    #region EF Stuff

    private const string _constructorDescription = "This service's code infrastructure does not assume devs using DataContext's constructors directly.";

#pragma warning disable CS8618 // This is EF, so.
    [Obsolete(_constructorDescription, false)]
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    [Obsolete(_constructorDescription, false)]
    public DataContext() : base() { }
#pragma warning restore CS8618

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Place custom DB models initialization here if needed.
    }

    #endregion
}