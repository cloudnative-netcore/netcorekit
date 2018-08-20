using System;

namespace NetCoreKit.Samples.TodoAPI.Dtos
{
  public class AuthorDto
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string GetFullName()
    {
      return $"{FirstName} {LastName}";
    }
  }
}
