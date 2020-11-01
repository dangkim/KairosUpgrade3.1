using Microsoft.Extensions.Caching.Distributed;
using Slot.Core.Services.Abstractions;
using Slot.Core.Data;
using Slot.Core.Extensions;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Slot.Core.Modules.Infrastructure.Models;

namespace Slot.Core.Services
{
    // TODO for funplay to move to FunplayBonusServcie
    public class BonusService : IBonusService
    {
        private readonly IDistributedCache cache;
        private readonly IDatabaseManager databaseManager;

        public BonusService(IDistributedCache cache,
                            IDatabaseManager databaseManager)
        {
            this.cache = cache;
            this.databaseManager = databaseManager;
        }

        public Task<BonusEntity> GetUnfinishBonus(UserSession userSession, int gameId)
        {
            if (userSession.IsFunPlay)
            {
                return cache.GetAsync<BonusEntity>(userSession.SessionKey);
            }
            return GetNonOptionalAndStartedOptionalBonusFromDatabase(userSession.UserId, gameId);
        }

        public Task<bool> UpdateBonus(UserSession userSession, BonusEntity bonus)
        {
            return bonus.UserId <= 0 ? UpdateBonusInCache(userSession, bonus) : UpdateBonusInDatabase(bonus);
        }

        public Task<bool> RemoveBonus(UserSession userSession, BonusEntity bonus)
        {
            return bonus.UserId <= 0 ? RemoveBonusFromCache(userSession) : RemoveBonusFromDatabase(bonus);
        }

        private Task<BonusEntity> GetBonusEntityFromCache(string guid)
        {
            return cache.GetAsync<BonusEntity>(guid);
        }

        private async Task<BonusEntity> GetBonusEntityFromDatabase(string guid, int userId, int gameId)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                return await db.Bonuses
                               .Where(b => b.UserId == userId)
                               .Where(b => b.GameId == gameId)
                               .Where(b => b.Guid == guid)
                               .AsNoTracking()
                               .FirstOrDefaultAsync();
            }
        }

        private async Task<BonusEntity> GetNonOptionalAndStartedOptionalBonusFromDatabase(int userId, int gameId)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                return await db.Bonuses
                               .Where(b => b.UserId == userId)
                               .Where(b => b.GameId == gameId)
                               .Where(b => ((b.IsOptional && b.IsStarted) || (!b.IsOptional)))
                               .AsNoTracking()
                               .FirstOrDefaultAsync();
            }
        }

        private async Task<bool> UpdateBonusInCache(UserSession userSession, BonusEntity bonus)
        {
            await cache.SetAsync(userSession.SessionKey, bonus, new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
            return true;
        }

        private async Task<bool> UpdateBonusInDatabase(BonusEntity bonus)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                var bonusEntity = new BonusEntity
                {
                    UserId = bonus.UserId,
                    GameId = bonus.GameId,
                    Guid = bonus.Guid,
                    Data = bonus.Data,
                    BonusType = bonus.BonusType,
                    Version = bonus.Version,
                    IsOptional = bonus.IsOptional,
                    IsStarted = bonus.IsStarted,
                    RoundId = bonus.RoundId,
                    BetReference = bonus.BetReference,
                    BnsClsId = bonus.BnsClsId,
                    ClientId = bonus.ClientId,
                    Order = bonus.Order,
                    IsFreeGame = bonus.IsFreeGame,
                    CampaignId = bonus.CampaignId
                };
                db.InsertOrUpdate(bonusEntity, bonus.UserId, bonus.GameId);
                return await db.SaveChangesAsync() > 0;
            }
        }

        private async Task<bool> RemoveBonusFromCache(UserSession userSession)
        {
            await cache.RemoveAsync<BonusEntity>(userSession.SessionKey);
            return true;
        }

        private async Task<bool> RemoveBonusFromDatabase(BonusEntity bonus)
        {
            using (var db = databaseManager.GetWritableDatabase())
            {
                var entity = db.Bonuses
                               .Where(x => x.UserId == bonus.UserId)
                               .Where(x => x.GameId == bonus.GameId)
                               .AsNoTracking()
                               .SingleOrDefault();
                if (entity != null)
                {
                    db.Bonuses.Remove(entity);
                    return await db.SaveChangesAsync() > 0;
                }
                return false;
            }
        }
    }
}
