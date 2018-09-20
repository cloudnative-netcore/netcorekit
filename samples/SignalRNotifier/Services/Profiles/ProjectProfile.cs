using System;
using AutoMapper;
using MediatR;
using Project.Proto;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Profiles
{
  public class ProjectProfile : Profile
  {
    public ProjectProfile()
    {
      // this.MapToNotification<ProjectCreatedMsg, Notifications.ProjectCreated>();
      CreateMap<TaskCreatedMsg, Notifications.TaskCreated>()
        .ForMember(x => x.Id, conf => conf.MapFrom(cg => new Guid(cg.Id)))
        .ForMember(x => x.ProjectId, conf => conf.MapFrom(cg => new Guid(cg.ProjectId)))
        .ForMember(x => x.OccurredOn, conf => conf.MapFrom(cg => cg.OccurredOn.ToDateTime()));

      CreateMap<ProjectCreatedMsg, Notifications.ProjectCreated>()
        .ForMember(x => x.Id, conf => conf.MapFrom(cg => new Guid(cg.Id)))
        .ForMember(x => x.OccurredOn, conf => conf.MapFrom(cg => cg.OccurredOn.ToDateTime()));

//      CreateMap<ProjectCreatedMsg, INotification>()
//        .As<Notifications.ProjectCreated>();
    }
  }
}
