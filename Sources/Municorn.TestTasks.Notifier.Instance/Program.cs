using Municorn.TestTasks.Notifier.Instance;

using IHost host = ConsoleHostBuilder.Build(args);

await host.RunAsync();