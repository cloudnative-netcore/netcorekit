using System.Threading.Tasks;
using NetCoreKit.Samples.TodoAPI.Dtos;

namespace NetCoreKit.Samples.TodoAPI.Domain
{
    public interface IUserGateway
    {
        Task<AuthorDto> GetAuthorAsync();
    }
}
