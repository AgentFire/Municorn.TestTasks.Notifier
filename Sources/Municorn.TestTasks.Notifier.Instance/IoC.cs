using Autofac;
using FluentValidation;
using HealthChecks.NpgSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Municorn.TestTasks.Notifier.BusinessLogic.Config;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Services;
using Municorn.TestTasks.Notifier.BusinessLogic.Validators;
using Municorn.TestTasks.Notifier.Data;
using Municorn.TestTasks.Notifier.Instance.AutoMapperProfilers;
using Municorn.TestTasks.Notifier.Instance.Configuration;
using Municorn.TestTasks.Notifier.Instance.Repositories;
using Municorn.TestTasks.Notifier.Instance.Services;

namespace Municorn.TestTasks.Notifier.Instance;

internal static class IoC
{
    internal static void RegisterServices(ContainerBuilder containerBuilder)
    {
        // Could go for IOptions here in case we'd opted for a hot reload.
        containerBuilder.RegisterConfigRecord<ConnectionStringsConfig>(configPath: "ConnectionStrings");
        containerBuilder.RegisterConfigRecord<FakesConfig>(configPath: "Fakes");
        containerBuilder.RegisterConfigRecord<ThrottleConfig>(configPath: "Throttle");

        containerBuilder
            .RegisterType<FireAndForgetService>()
            .As<IFireAndForgetService>()
            .As<IHostedService>()
            .SingleInstance();

        containerBuilder
            .Register(context => new NpgSqlHealthCheck(context.Resolve<ConnectionStringsConfig>().ServiceDb, "SELECT 1;"))
            .AsSelf()
            .SingleInstance();

        containerBuilder
            .RegisterType<StartupService>()
            .As<IHostedService>()
            .SingleInstance();

        DiscoverAndRegisterNotificationSenders(containerBuilder);
    }

    private static void DiscoverAndRegisterNotificationSenders(ContainerBuilder containerBuilder)
    {
        var infos = from impl in typeof(INotificationSender<>).Assembly.DefinedTypes
                    where impl.IsClass
                    where !impl.IsAbstract
                    where !impl.IsInterface
                    from @interface in impl.GetInterfaces()
                    where @interface.IsGenericType
                    let interfaceDefinition = @interface.GetGenericTypeDefinition()
                    where interfaceDefinition.IsAssignableFrom(typeof(INotificationSender<>))
                    select new
                    {
                        ImplementationType = impl,
                        InterfaceType = @interface
                    };

        foreach (var data in infos)
        {
            containerBuilder
                .RegisterType(data.ImplementationType)
                .As(data.InterfaceType)
                .SingleInstance();
        }
    }

    internal static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions(); // ASP.NET Core requires Options to run.
        RegisterDbContext(services);

        services.AddLogging(T =>
        {
            T.AddConsole().AddConfiguration(configuration.GetSection("Logging"));
        });

        services.AddAutoMapper(typeof(NotificationsProfile));
        services.AddValidatorsFromAssembly(typeof(NotificationValidatorBase<>).Assembly);

        services.AddSingleton<IValidatorFactory, ServiceProviderValidatorFactory>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddTransient<INotificationsRepository, NotificationsRepository>();
    }

    private static void RegisterDbContext(IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<DataContext>((serviceProvider, options) =>
        {
            var fakes = serviceProvider.GetRequiredService<FakesConfig>();

            if (fakes.Database)
            {
                options.UseInMemoryDatabase("FakeDb");
            }
            else
            {
                var connStrings = serviceProvider.GetRequiredService<ConnectionStringsConfig>();

                options.UseNpgsql(connStrings.ServiceDb, options =>
                {
                    options.MigrationsAssembly(typeof(DataContext).Assembly.FullName);
                });
            }

            // To remove excessive logging until we need to debug the SQL generation.
            options.UseLoggerFactory(NullLoggerFactory.Instance);
        }, ServiceLifetime.Scoped);
    }
}
