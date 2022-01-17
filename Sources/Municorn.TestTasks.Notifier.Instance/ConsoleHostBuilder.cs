using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.NpgSql;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Municorn.TestTasks.Notifier.Instance.Web.Controllers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Municorn.TestTasks.Notifier.Instance;

public static class ConsoleHostBuilder
{
    public static IHost Build(params string[] args)
    {
        IHostBuilder hostBuilder = new HostBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((hostContext, cfg) =>
            {
                cfg.SetBasePath(Directory.GetCurrentDirectory());

                // Default web port set via the ASP.NET's default configuration key.
                cfg.AddInMemoryCollection(("urls", "http://*:8075"));

                cfg.AddJsonFile("appsettings.json", optional: false);
                // Can add appsettings.Environment.json here.
                cfg.AddCommandLine(args.ToArray());
                // Can also add AddEnvironmentVariables here. Or anything else in the desired precedence.
            })
            // Register your dependencies via some powerful container library, for instance, Autofac.
            .ConfigureContainer<ContainerBuilder>(container =>
            {
                IoC.RegisterServices(container);
            })
            // Or instead do it via default Microsoft's ServiceCollection. Or do it both ways, since it's entirely possible.
            .ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.Configure<HostOptions>(hostOptions =>
                {
                    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
                });

                IoC.RegisterServices(serviceCollection, context.Configuration);
            })
            // We need a web api server on this microservice, so...
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .ConfigureServices(services =>
                    {
                        services
                            .AddHealthChecks()
                            .Add(new HealthCheckRegistration("Database", serviceProvider => serviceProvider.GetRequiredService<NpgSqlHealthCheck>(), HealthStatus.Degraded, null));

                        services.AddRouting();
                        services.AddControllersCorrectlyFrom(typeof(NotifierController).Assembly);
                        services.AddSwaggerGen();
                        services.Configure<JsonSerializerOptions>(options =>
                        {
                            options.Converters.Add(new JsonStringEnumConverter());
                        });

                        // Add authentication, authorization or any other web service here.
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();

                        app
                            .UseSwagger()
                            .UseSwaggerUI(c =>
                            {
                                c.SwaggerEndpoint("v1/swagger.json", "Notification API");
                                c.RoutePrefix = "swagger";
                            });

                        // Use authentication, authorization or any other web service here.

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                            endpoints.MapHealthChecks("/health");
                        });
                    })
                    .UseKestrel();
            }, T => T.SuppressEnvironmentConfiguration = true);

        return hostBuilder
            .UseConsoleLifetime()
            .Build();
    }
}
