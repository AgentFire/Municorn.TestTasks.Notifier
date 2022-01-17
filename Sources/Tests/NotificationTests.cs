using AutoMapper;
using AutoMapper.Internal;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using Municorn.TestTasks.Notifier.Instance;
using Municorn.TestTasks.Notifier.Instance.Web.Controllers;
using Municorn.TestTasks.Notifier.Instance.Web.Models.Out;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace Tests;

public sealed class NotificationTests : IDisposable
{
    #region TypesData

    private sealed class TypesData : IEnumerable<object[]>
    {
        private static readonly Type[] _definedNotifications =
            (from type in typeof(INotification).Assembly.DefinedTypes
             where type.IsClass
             where !type.IsInterface
             where !type.IsAbstract
             where type.IsAssignableTo(typeof(INotification))
             select type).ToArray();

        public IEnumerator<object[]> GetEnumerator() => _definedNotifications.Select(T => new object[] { T }).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    #endregion

    private readonly IHost _hostInstance = ConsoleHostBuilder.Build();

    [Theory]
    [ClassData(typeof(TypesData))]
    public void WeHaveValidators(Type notificationType)
    {
        _hostInstance.Services.GetService(typeof(IValidator<>).MakeGenericType(notificationType)).ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(TypesData))]
    public void WeHaveSenders(Type notificationType)
    {
        _hostInstance.Services.GetService(typeof(INotificationSender<>).MakeGenericType(notificationType)).ShouldNotBeNull();
    }

    [Theory]
    [ClassData(typeof(TypesData))]
    public void WeHaveMapper(Type notificationType)
    {
        IGlobalConfiguration config = (IGlobalConfiguration)_hostInstance.Services.GetRequiredService<IConfigurationProvider>();
        config.GetAllTypeMaps().Where(T => T.DestinationType == notificationType).Any().ShouldBeTrue();
    }

    [Theory]
    [ClassData(typeof(TypesData))]
    public void ControllerHasMethods(Type notificationType)
    {
        IGlobalConfiguration config = (IGlobalConfiguration)_hostInstance.Services.GetRequiredService<IConfigurationProvider>();
        IEnumerable<Type> dtoCandidates = config.GetAllTypeMaps().Where(T => T.DestinationType == notificationType).Select(T => T.SourceType);

        foreach (Type dto in dtoCandidates)
        {
            MethodInfo? method = typeof(NotifierController).GetMethod("Post", new[] { dto, typeof(CancellationToken) });

            if (method is null)
            {
                continue;
            }

            Assert.True(
                method.ReturnType == typeof(CreateNotificationResult)
                || method.ReturnType == typeof(Task<CreateNotificationResult>)
                || method.ReturnType == typeof(ValueTask<CreateNotificationResult>)
            );

            // One is enough.
            return;
        }

        throw new XunitException($"No suitable controller method was found for the {notificationType.Name} notification type");
    }

    public void Dispose()
    {
        _hostInstance.Dispose();
    }
}