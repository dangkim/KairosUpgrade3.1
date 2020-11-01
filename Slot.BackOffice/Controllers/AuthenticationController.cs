using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Slot.BackOffice.Configs.Authentication;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.HttpClients;
using Slot.Core.Data;
using Slot.Core.Data.Views.Authentication;
using Slot.Model.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Slot.BackOffice.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    public class AuthenticationController : BaseController
    {
        private readonly IDatabaseManager databaseManager;
        private readonly AuthenticationConfig authenticationConfig;
        private readonly CasClient casClient;

        public AuthenticationController(
            IDatabaseManager databaseManager,
            IOptions<AuthenticationConfig> authenticationConfig,
            CasClient casClient)
        {
            this.databaseManager = databaseManager;
            this.authenticationConfig = authenticationConfig.Value;
            this.casClient = casClient;
        }

        [HttpGet]
        [Route("/api/[controller]")]
        public async Task<IActionResult> SecurityCheck(string ticket)
        {
            var response = await casClient.DoSecurityCheck(ticket);
            var casResponse = CasSecurityCheckResponse.Deserialize(response);

            if (casResponse?.IsSuccess == true)
            {
                using (var db = databaseManager.GetWritableDatabase())
                {
                    var @operator = await GetOperator(casResponse.Operator, db);

                    if (@operator == null)
                    {
                        return new Data.Responses.BadRequestResult(string.Empty, Model.ErrorCode.AccessDenied);
                    }
                    else
                    {
                        var account = await AuthenticateMember(@operator, casResponse, db);

                        var jwtUser = new JwtUser(account);
                        var token = jwtUser.GetToken(
                                                authenticationConfig.Jwt.Key,
                                                authenticationConfig.Jwt.Issuer,
                                                authenticationConfig.Jwt.Issuer,
                                                authenticationConfig.Jwt.Duration);

                        return GetResult(token);
                    }
                }
            }
            else
            {
                return GetResult(Model.ErrorCode.AccessDenied);
            }
        }

        private async Task<Operator> GetOperator(string operatorName, IReadOnlyDatabase db)
        {
            var @operator = operatorName.ToLowerInvariant();

            var exceptions = new List<string> { "w88", "w88all" };

            return !exceptions.Contains(@operator) ?
                            await db.Operators.FirstOrDefaultAsync(o => o.Name == @operator) :
                            await db.Operators.FirstOrDefaultAsync(o => o.Tag == "w88");
        }

        private async Task<Account> AuthenticateMember(Operator @operator, CasSecurityCheckResponse casResponse, IWritableDatabase db)
        {
            var userName = casResponse.User.ToLowerInvariant();
            var account = await db.Accounts
                                    .Include(acc => acc.Role)
                                    .Include(acc => acc.Operator)
                                    .FirstOrDefaultAsync(acc => acc.Username == userName && acc.OperatorId == @operator.Id);

            if (account != null)
            {
                account.LastLoginUtc = DateTime.UtcNow;
            }
            else
            {
                var defaultRole = await db.Roles.FirstOrDefaultAsync(role => role.Id == (int)Roles.Minimum);
                var taipeiUtc = await db.UtcTimeOffsets.FirstOrDefaultAsync(utc => utc.Offset == "+08:00");

                account = new Account
                {
                    Username = userName,
                    RealName = userName,
                    RoleId = defaultRole.Id,
                    OperatorId = @operator.Id,
                    UtcTimeOffsetId = taipeiUtc.Id,
                    Active = true,
                    LastLoginUtc = DateTime.UtcNow
                };
            }

            db.InsertOrUpdate(account, account.Id);
            await db.SaveChangesAsync();

            return account;
        }
    }
}