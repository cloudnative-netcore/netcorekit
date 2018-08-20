using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.Extensions
{
  public static class TaskExtensions
  {
    public static TaskDto ToDto(this Task task)
    {
      return new TaskDto
      {
        Id = task.Id,
        Title = task.Title,
        Completed = task.Completed ?? false,
        Order = task.Order ?? 1,
        AuthorName = task.AuthorName
      };
    }
  }
}
