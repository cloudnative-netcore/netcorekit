using NetCoreKit.Samples.TodoAPI.Domain;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.Extensions
{
	public static class TodoExtensions
	{
		public static TodoDto ToDto(this Todo todo)
		{
			return new TodoDto
			{
				Id = todo.Id,
				Title = todo.Title,
				Completed = todo.Completed ?? false,
				Order = todo.Order ?? 1
			};
		}
		}
}
