using AutoMapper;
using NetCoreKit.Infrastructure.Mappers;
using Project.Proto;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Profiles
{
  public class ProjectProfile : Profile
  {
    public ProjectProfile()
    {
      this.MapToNotification<ProjectCreatedMsg, Notifications.ProjectCreated>();
      //this.MapToNotification<TaskCreated, Notifications.TaskCreated>();
    }
  }
}
