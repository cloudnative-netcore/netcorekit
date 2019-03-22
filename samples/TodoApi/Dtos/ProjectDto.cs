using System;
using System.Collections.Generic;

namespace NetCoreKit.Samples.TodoAPI.Dtos
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<TaskDto> Tasks { get; set; } = new List<TaskDto>();
    }
}
