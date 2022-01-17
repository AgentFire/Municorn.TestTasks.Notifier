using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Municorn.TestTasks.Notifier.BusinessLogic.Contracts;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using Municorn.TestTasks.Notifier.Data;
using Municorn.TestTasks.Notifier.Data.Models;
using Municorn.TestTasks.Notifier.Instance.Web.Models.In;
using Municorn.TestTasks.Notifier.Instance.Web.Models.Out;

namespace Municorn.TestTasks.Notifier.Instance.Web.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class NotifierController : ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public NotifierController(DataContext dataContext, IMapper mapper, INotificationService notificationService)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    [HttpGet("{id}/status")]
    public ValueTask<NotificationStatus> Get(Guid id, CancellationToken cancellationToken)
    {
        return _notificationService.GetStatus(id, cancellationToken);
    }

    private async ValueTask<CreateNotificationResult> PostNotificationIfNew<T>(T dto, CancellationToken cancellationToken) where T : INotification
    {
        Guid? requestId = null;

        // Idempotency implementation example.
        if (Guid.TryParse(Request.Headers.RequestId.SingleOrDefault(), out Guid requestIdValue))
        {
            requestId = requestIdValue;
        }

        if (requestId is not null)
        {
            NotificationRequest? request = await _dataContext
                .NotificationRequests
                .Where(T => T.ResultNotificationId == requestId)
                .SingleOrDefaultAsync(cancellationToken);

            if (request is not null)
            {
                return new CreateNotificationResult(request.ResultNotificationId);
            }
        }

        Guid notificationId = await _notificationService.Post(dto, cancellationToken);

        if (requestId is not null)
        {
            _dataContext.NotificationRequests.Add(new NotificationRequest(requestId.Value, notificationId));
            await _dataContext.SaveChangesAsync(cancellationToken);
        }

        return new CreateNotificationResult(notificationId);
    }

    [Route("ios")]
    [HttpPost]
    public ValueTask<CreateNotificationResult> Post([FromBody] IosNotificationDto dto, CancellationToken cancellationToken)
    {
        return PostNotificationIfNew(_mapper.Map<IosNotification>(dto), cancellationToken);
    }

    [Route("android")]
    [HttpPost]
    public ValueTask<CreateNotificationResult> Post([FromBody] AndroidNotificationDto dto, CancellationToken cancellationToken)
    {
        return PostNotificationIfNew(_mapper.Map<AndroidNotification>(dto), cancellationToken);
    }
}
