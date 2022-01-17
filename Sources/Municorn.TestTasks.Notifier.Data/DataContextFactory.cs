using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.ComponentModel;
using System.Diagnostics;

namespace Municorn.TestTasks.Notifier.Data;

[DebuggerNonUserCode]
[EditorBrowsable(EditorBrowsableState.Never)]
[Obsolete("This factory is only required for the dotnet-ef tool", true)]
public sealed class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>().UseNpgsql("Host=.;");

        return new DataContext(optionsBuilder.Options);
    }
}
