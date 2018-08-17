using System;

namespace NetCoreKit.Samples.TodoAPI.Dtos
{
	public class TodoDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public int Order { get; set; }
		public bool Completed { get; set; }
	}
}
