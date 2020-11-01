using Slot.Core.Services.Models;
using Slot.Model;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticateResult, ErrorCode>> Authenticate(string merchant, string token);
    }
}
