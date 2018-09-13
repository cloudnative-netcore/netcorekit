using AutoMapper;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.Contracts.Events;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Profiles
{
  public class ProjectProfile : Profile
  {
    public ProjectProfile()
    {
      this.MapToNotification<ProjectCreated, Notifications.ProjectCreated>();
      this.MapToNotification<TaskCreated, Notifications.TaskCreated>();
    }
  }
}
