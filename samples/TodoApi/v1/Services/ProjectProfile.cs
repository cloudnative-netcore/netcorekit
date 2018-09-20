using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using NetCoreKit.Samples.TodoAPI.Domain;
using Project.Proto;

namespace NetCoreKit.Samples.TodoAPI.v1.Services
{
  public class ProjectProfile : Profile
  {
    public ProjectProfile()
    {
      CreateMap<ProjectCreated, ProjectCreatedMsg>()
        .ForMember(
          x => x.Key,
          conf => conf.MapFrom(cg => cg.GetType().Name))
        .ForMember(
          x => x.Id,
          conf => conf.MapFrom(cg => cg.Id.ToString()))
        .ForMember(
          x => x.OccurredOn,
          conf => conf.MapFrom(cg => Timestamp.FromDateTime(cg.OccurredOn)));

      CreateMap<TaskCreated, TaskCreatedMsg>()
        .ForMember(
          x => x.Key,
          conf => conf.MapFrom(cg => cg.GetType().Name))
        .ForMember(
          x => x.Id,
          conf => conf.MapFrom(cg => cg.Id.ToString()))
        .ForMember(
          x => x.ProjectId,
          conf => conf.MapFrom(cg => cg.ProjectId.ToString()))
        .ForMember(
          x => x.OccurredOn,
          conf => conf.MapFrom(cg => Timestamp.FromDateTime(cg.OccurredOn)));
    }
  }
}
