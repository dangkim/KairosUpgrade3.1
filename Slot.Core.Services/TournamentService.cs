using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Data.Models;
using Slot.Core.Services.Abstractions;
using Slot.Model;
using Slot.Model.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Slot.Core.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly IDatabaseManager databaseManager;
        private readonly CachedSettings cachedSettings;
        private readonly ILogger<TournamentService> logger;

        public TournamentService(IDatabaseManager databaseManager,
                                 CachedSettings cachedSettings,
                                 ILogger<TournamentService> logger)
        {
            this.databaseManager = databaseManager;
            this.cachedSettings = cachedSettings;
            this.logger = logger;
        }

        public async Task<Result<TournamentInfo, ErrorCode>> GetInfo(string merchant, string gameName)
        {
            if (!cachedSettings.OperatorsByName.TryGetValue(merchant, out Operator op))
            {
                return ErrorCode.AccessDenied;
            }

            if (!cachedSettings.GamesByName.TryGetValue(gameName, out Game game))
            {
                return ErrorCode.AccessDenied;
            }

            using (var db = databaseManager.GetWritableDatabase())
            {
                var gameId = new SqlParameter("gameId", game.Id);
                var opId = new SqlParameter("opId", op.Id);
                var sql = @"SELECT 
	                            id = t.Id,
		                        type =  CASE  mr.RelationId WHEN NULL THEN 'internal' 
			                            ELSE 'global'
			                            END
                                FROM dbo.Tournament t WITH (NOLOCK)
	                            INNER JOIN dbo.TRelation gr WITH (NOLOCK) ON t.Id = gr.TournamentId AND gr.RelationType = 2
	                            LEFT JOIN dbo.TRelation mr WITH (NOLOCK) ON t.Id = mr.TournamentId AND mr.RelationType = 6
	
                            WHERE
		                        t.IsCancelled = 0 		    
	                            AND gr.RelationId = @gameId
	                            AND GETUTCDATE() >= t.StartTime AND GETUTCDATE() < t.EndTime
                                AND ISNULL(mr.RelationId, t.OperatorId) = @opId";
                return await db.TournamentInfo.FromSql(sql, gameId, opId).SingleOrDefaultAsync();
            }
        }
    }
}
