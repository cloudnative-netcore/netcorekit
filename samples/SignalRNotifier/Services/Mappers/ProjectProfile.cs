using System;
using AutoMapper;
using NetCoreKit.Samples.SignalRNotifier.Services.Events;
using Project.Proto;

namespace NetCoreKit.Samples.SignalRNotifier.Services.Mappers
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<TaskCreatedMsg, TaskCreated>()
                .ForMember(x => x.Id, conf => conf.MapFrom(cg => new Guid(cg.Id)))
                .ForMember(x => x.ProjectId, conf => conf.MapFrom(cg => new Guid(cg.ProjectId)))
                .ForMember(x => x.OccurredOn, conf => conf.MapFrom(cg => cg.OccurredOn.ToDateTime()));

            CreateMap<ProjectCreatedMsg, ProjectCreated>()
                .ForMember(x => x.Id, conf => conf.MapFrom(cg => new Guid(cg.Id)))
                .ForMember(x => x.OccurredOn, conf => conf.MapFrom(cg => cg.OccurredOn.ToDateTime()));
        }
    }
}
