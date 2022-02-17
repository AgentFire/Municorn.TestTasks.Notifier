using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Municorn.TestTasks.Notifier.BusinessLogic.Config;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using Municorn.TestTasks.Notifier.Instance;
using Municorn.TestTasks.Notifier.Instance.Web.Controllers;
using Municorn.TestTasks.Notifier.Instance.Web.Models.In;
using Municorn.TestTasks.Notifier.Instance.Web.Models.Out;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public sealed class IntegrationTests
{
    [Theory]
    [InlineData("TokenValue1", "MessageText1")]
    [InlineData("TokenValue2", "MessageText2")]
    public async Task NotificationGetsSent(string tokenValue, string messageText)
    {
        using IHost _hostInstance = ConsoleHostBuilder.Build();
        CreateNotificationResult createResult;

        using (IServiceScope requestScope = _hostInstance.Services.CreateScope())
        {
            NotifierController controller = _hostInstance.Services.GetRequiredService<NotifierController>();

            var httpRequest = A.Fake<HttpRequest>();
            var httpContext = A.Fake<HttpContext>();
            A.CallTo(() => httpContext.Request).Returns(httpRequest);
            A.CallTo(() => httpRequest.Headers).Returns(new HeaderDictionary());
            controller.ControllerContext = new ControllerContext(new ActionContext(httpContext, A.Fake<RouteData>(), new ControllerActionDescriptor()));

            createResult = await controller.Post(new IosNotificationDto(tokenValue, messageText), CancellationToken.None);

            createResult.ShouldNotBeNull();
            createResult.NotificationId.ShouldNotBe(Guid.Empty);
            createResult.NotificationRequestState.ShouldBe(NotificationRequestStateDto.NotDelivered);
        }

        // Wait until the delivery.
        await Task.Delay(_hostInstance.Services.GetRequiredService<ThrottleConfig>().MaximumSendTime + TimeSpan.FromSeconds(1));

        using (IServiceScope requestScope = _hostInstance.Services.CreateScope())
        {
            NotifierController controller = _hostInstance.Services.GetRequiredService<NotifierController>();

            NotificationStatus status = await controller.Get(createResult.NotificationId, CancellationToken.None);

            status.ShouldBe(NotificationStatus.Delivered);
        }
    }
}