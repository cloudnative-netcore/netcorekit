using AutoMapper;
using NetCoreKit.Infrastructure.Mappers;
using NetCoreKit.Samples.Contracts.Events;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Profiles
{
  public class ProjectProfile : Profile
  {
    public ProjectProfile()
    {
      this.MapMySelf<ProjectCreated>();
      this.MapMySelf<TaskCreated>();
    }
  }
}
