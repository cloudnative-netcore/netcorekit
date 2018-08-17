using System;
using System.ComponentModel.DataAnnotations;
using NetCoreKit.Domain;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
	public sealed class Todo : EntityBase
	{
		internal Todo(string title)
			: this(Guid.NewGuid(), title)
		{
		}

		public Todo(Guid id, string title) : base(id)
		{
			Title = title;
		}

		public static Todo Load(string title)
		{
			return new Todo(title);
		}

		public static Todo Load(Guid id, string title)
		{
			return new Todo(id, title);
		}

		public int? Order { get; private set; } = 1;
		[Required] public string Title { get; private set; }
		public bool? Completed { get; private set; } = false;

	  public Todo ChangeTitle(string title)
	  {
      if(string.IsNullOrEmpty(title))
        throw new DomainException("Order is null or empty.");
	    Title = title;
	    return this;
	  }

    public Todo ChangeOrder(int order)
		{
			if (order <= 0)
				throw new DomainException("Order could be greater than zero.");
			Order = order;
			return this;
		}

		public Todo ChangeToCompleted()
		{
			Completed = true;
			return this;
		}
	}
}
