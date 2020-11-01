using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using System.Threading.Tasks;

namespace Slot.Core.Services.Abstractions
{
    public interface IUserService
    {
        Task<Result<AuthenticateResult, ErrorCode>> Authenticate(string operatorId, string token, string ipAddress);
        Task<Result<string, ErrorCode>> GetFunplayKey(string op);
        Task<UserSession> GetUserSession(string sessionKey);
        Task<Result<GetBalanceResult, ErrorCode>> GetBalance(string token, string game, PlatformType platform);

        Task<UserGameState> GetGameState<T>(RequestContext<T> requestContext);
        Task<bool> UpdateGameState<T>(RequestContext<T> requestContext, UserGameState gameState);

        Task<UserGameData> GetUserGameData(long userId, int gameId);
        Task<bool> UpdateUserGameData(UserGameData userGameData);

        Task<int> GetLevel(UserGameKey userGameKey);
        Task<UserGameSpinData> GetLastSpinData(UserSession userSession, Game game);
        Task UpdateLastSpinData(UserSession userSession, Game game, SpinResult spinResult);

        Task<IWalletService> GetWallet(UserSession userSession);
    }
}
