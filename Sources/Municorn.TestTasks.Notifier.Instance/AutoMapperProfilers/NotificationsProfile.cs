using AutoMapper;
using Municorn.TestTasks.Notifier.BusinessLogic.Models;
using Municorn.TestTasks.Notifier.Instance.Web.Models.In;

namespace Municorn.TestTasks.Notifier.Instance.AutoMapperProfilers;

public sealed class NotificationsProfile : Profile
{
    public NotificationsProfile()
    {
        CreateMap<AndroidNotificationDto, AndroidNotification>();
        CreateMap<IosNotificationDto, IosNotification>();
    }
}
