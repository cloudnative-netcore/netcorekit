using System.Collections.Generic;
using System.Linq;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.Extensions
{
    public static class ProjectExtensions
    {
        public static ProjectDto ToDto(this Domain.Project prj)
        {
            return new ProjectDto
            {
                Id = prj.Id,
                Name = prj.Name,
                Tasks = prj.Tasks == null
                    ? new List<TaskDto>()
                    : prj.Tasks.Select(t => t.ToDto()).ToList()
            };
        }
    }
}
