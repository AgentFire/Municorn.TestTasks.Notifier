using Autofac;
using System.Reflection;

namespace Municorn.TestTasks.Notifier.Instance;

internal static class Extensions
{
    /// <summary>
    /// Shortcut for registering configuration DTOs.
    /// </summary>
    internal static void RegisterConfigRecord<T>(this ContainerBuilder builder, string configPath, Func<IComponentContext, T>? defaultValueResolver = null) where T : class
    {
        builder
            .Register(componentContext =>
            {
                T? value = componentContext
                    .Resolve<IConfiguration>()
                    .GetSection(configPath)
                    .GetViaConstructor<T>();

                return value ?? defaultValueResolver?.Invoke(componentContext) ?? throw new Exception("Cannot initialize null");
            })
            .AsSelf()
            .SingleInstance();
    }
    /// <summary>
    /// Allows to use any <see langword="record"/> as a proper configuration DTO.
    /// </summary>
    internal static T GetViaConstructor<T>(this IConfiguration config)
    {
        try
        {
            // Default handler.
            return config.Get<T>();
        }
        catch (InvalidOperationException) when (typeof(T).IsClass)
        {
            // Handle classes with no parameterless constructor.
            var ctors = from constructors in typeof(T).GetConstructors()
                        let parameters = constructors.GetParameters()
                        orderby parameters.Length ascending
                        select new
                        {
                            Constructor = constructors,
                            Params = parameters
                        };

            var ctor = ctors.FirstOrDefault() ?? throw new ArgumentException($"Can't find public constructor for type {typeof(T).FullName}");
            var argList = new List<object?>(ctor.Params.Length);

            foreach (ParameterInfo pi in ctor.Params)
            {
                MethodInfo recur = typeof(Extensions).GetMethod(nameof(GetViaConstructor), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)!.MakeGenericMethod(pi.ParameterType);
                object? recurResult = recur.Invoke(null, new object[] { config.GetSection(pi.Name) });
                argList.Add(recurResult);
            }

            return (T)ctor.Constructor.Invoke(argList.ToArray());
        }
    }
    internal static IConfigurationBuilder AddInMemoryCollection(this IConfigurationBuilder builder, params (string ConfigPath, string Value)[] values)
    {
        return builder.AddInMemoryCollection(values.Select(T => new KeyValuePair<string, string>(T.ConfigPath, T.Value)));
    }
    /// <summary>
    /// A framework's bug workaround.
    /// </summary>
    internal static void AddControllersCorrectlyFrom(this IServiceCollection serviceCollection, Assembly assembly)
    {
        IWebHostEnvironment? hostInvironment = serviceCollection
            .Where(T => typeof(IWebHostEnvironment).IsAssignableFrom(T.ServiceType))
            .Single()
            .ImplementationInstance as IWebHostEnvironment;

        string? appName = null;

        if (hostInvironment is not null)
        {
            appName = hostInvironment.ApplicationName;
            hostInvironment.ApplicationName = assembly.GetName().Name;
        }

        try
        {
            // There is a bug in the current ASP.NET Core's code. For some obsolete reason it reaches out for ApplicationName and considers it this assembly's name.
            // If an ApplicationName is altered (lets say via this service's configuration), the standard AddControllers method will crash the process.
            // (https://github.com/dotnet/aspnetcore/blob/24dacc25357988dbe699e72580a8ea569a33b4c4/src/Mvc/Mvc.Core/src/DependencyInjection/MvcCoreServiceCollectionExtensions.cs#L85)

            serviceCollection
                .AddControllers()
                .AddControllersAsServices();
        }
        finally
        {
            if (hostInvironment is not null)
            {
                hostInvironment.ApplicationName = appName;
            }
        }
    }
}
