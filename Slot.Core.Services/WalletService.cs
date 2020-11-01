using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Extensions;
using Slot.Core.Modules.Infrastructure.Models;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Exceptions;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using Slot.Model.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slot.Core.Services
{
    public class WalletService : IWalletService
    {
        private const string URL_WALLETPROXY = "URL_WALLETPROXY";
        private readonly IDatabaseManager databaseManager;
        private readonly ILogger<WalletService> logger;

        private readonly string proxyUrl;
        private readonly string extraInfo;
        private readonly UserSession userSession;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly bool enableEndGame;

        public WalletService(UserSession userSession,
                             IHttpClientFactory httpClientFactory,
                             CachedSettings cachedSettings,
                             IDatabaseManager databaseManager,
                             ILogger<WalletService> logger)
        {
            this.userSession = userSession;
            this.httpClientFactory = httpClientFactory;
            this.databaseManager = databaseManager;
            this.logger = logger;

            if (cachedSettings.ConfigSettings.TryGetValue(URL_WALLETPROXY, out ConfigurationSetting so))
            {
                proxyUrl = so.Value;
            }
            else
            {
                using (var db = databaseManager.GetReadOnlyDatabase())
                {
                    if (string.IsNullOrEmpty(proxyUrl))
                    {
                        proxyUrl = db.ConfigurationSettings
                                     .Where(s => s.Name == URL_WALLETPROXY)
                                     .AsNoTracking()
                                     .FirstOrDefault().Value;
                    }
                }
            }
            extraInfo = userSession.ExtraInfo ?? string.Empty;
            if (cachedSettings.OperatorsById.TryGetValue(userSession.User.OperatorId, out Operator op))
            {
                enableEndGame = op.EnableEndGame;
            }
        }

        public async Task<WalletResult> Credit(decimal amount, int gameId, long transactionId, decimal jwin, long roundId, string betId, int platform, string debitTrxId, string refTrxId, bool isEndGame = false)
        {
            if (amount < 0)
                throw new ArgumentException(@"Amount must be positive or 0.", "amount");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var user = userSession.User;

            var wt = new WalletTransaction
            {
                Guid = Guid.NewGuid().ToString(),
                Type = WalletTransactionType.Add,
                Amount = amount,
                GameTransactionId = transactionId,
                WalletProviderId = user.Operator.WalletProviderId
            };

            try
            {
                var url = String.Format("{0}/wallet/credit", proxyUrl);

                var prm = String.Format("operator={0}&cust_id={1}&currency={2}&trx_id={3}&game_id={4}&amount={5}&jackpot_win={6}&round_id={7}&bet_id={8}&platform={9}&isEndGame={10}&extra_info={11}&debit_trx_id={12}&operationcode={13}",
                    user.Operator.Tag,
                    user.ExternalId,
                    user.Currency.IsoCode,
                    wt.Guid,
                    gameId,
                    amount,
                    jwin,
                    roundId,
                    betId,
                    platform,
                    isEndGame ? 1 : 0,
                    extraInfo,
                    debitTrxId,
                    transactionId);

                var doc = await GetXmlAsync(url, prm);
                //var doc = HttpUtil.GetResponseXml(url, prm);

                var root = doc.Element("resp");
                if (root == null)
                {
                    logger.LogError("Credit unsuccessful. Can't find `resp` as root element, probably incrrect format.");
                    logger.LogError(doc.ToString());
                    throw new WalletException("Credit unsuccessful. Root element not found");
                }

                var wr = new WalletResult
                {
                    Response = doc.ToString(),
                    ErrorCode = Convert.ToInt32(root.Element("error_code").Value)
                };
                if (wr.ErrorCode != 0)
                {
                    logger.LogError("Credit unsuccessful. Api return error code {errorCode}", wr.ErrorCode);
                    throw new WalletException("Credit unsuccessful. Api return error code " + wr.ErrorCode);
                }

                wr.Balance = Convert.ToDecimal(root.Element("after").Value);
                wr.ExchangeRate = Convert.ToDecimal(root.Element("exchange_rate").Value);
                wr.TransactionId = wt.WalletProviderTransactionId = root.Element("trx_id").Value;

                return wr;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                wt.IsError = true;
                wt.ErrorMessage = ex.ToString();
                throw new WalletException(ex.Message, ex);
            }
            finally
            {
                stopWatch.Stop();
                wt.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                await SaveWalletTransation(wt);
            }
        }

        private async Task SaveWalletTransation(WalletTransaction walletTransaction)
        {
            try
            {
                using (var db = databaseManager.GetWritableDatabase())
                {
                    db.WalletTransactions.Add(walletTransaction);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                using (logger.BeginScope("Save WalletTransaction fault"))
                {
                    logger.LogError(ex, ex.Message);
                    logger.LogInformation(JsonHelper.ToString(walletTransaction));
                }
            }
        }

        public async Task<WalletResult> Debit(decimal amount, int gameId, long transactionId, decimal jcon, long roundId, int platform, long prevRoundId)
        {
            if (amount < 0)
                throw new ArgumentException(@"Amount must be positive or 0.", "amount");

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var user = userSession.User;
            var wt = new WalletTransaction
            {
                Guid = Guid.NewGuid().ToString(),
                Type = WalletTransactionType.Deduct,
                Amount = amount,
                GameTransactionId = transactionId,
                WalletProviderId = user.Operator.WalletProviderId
            };

            try
            {
                var url = String.Format("{0}/wallet/debit", proxyUrl);

                var prm = String.Format("operator={0}&cust_id={1}&currency={2}&trx_id={3}&game_id={4}&amount={5}&jackpot_contribution={6}&round_id={7}&platform={8}&prev_round_id={9}&extra_info={10}&operationcode={11}",
                    user.Operator.Tag,
                    user.ExternalId,
                    user.Currency.IsoCode,
                    wt.Guid,
                    gameId,
                    amount,
                    jcon,
                    roundId,
                    platform,
                    prevRoundId,
                    extraInfo,
                    transactionId);

                //var doc = HttpUtil.GetResponseXml(url, prm);
                var doc = await GetXmlAsync(url, prm);
                var root = doc.Element("resp");
                if (root == null)
                {
                    throw new WalletException("Debit unsuccessful. root element not found");
                }

                var wc = new WalletResult
                {
                    Response = doc.ToString(),
                    ErrorCode = Convert.ToInt32(root.Element("error_code").Value)
                };
                if (wc.ErrorCode == -4)
                {
                    throw new InsufficientBalanceException();
                }
                if (wc.ErrorCode != 0)
                {
                    logger.LogError("Debit unsuccessful. Api return error code {errorCode}", wc.ErrorCode);
                    throw new WalletException("Debit Unsuccessful. Api return error code " + wc.ErrorCode);
                }

                wc.Balance = Convert.ToDecimal(root.Element("after").Value);
                wc.ExchangeRate = Convert.ToDecimal(root.Element("exchange_rate").Value);
                wc.TransactionId = wt.WalletProviderTransactionId = root.Element("trx_id").Value;

                var betId = root.Element("bet_id");
                if (betId != null) wc.BetReference = betId.Value;

                wc.Guid = wt.Guid;

                return wc;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                wt.IsError = true;
                wt.ErrorMessage = ex.ToString();
                throw new WalletException(ex.Message, ex);
            }
            finally
            {
                stopWatch.Stop();
                wt.ElapsedSeconds = stopWatch.Elapsed.TotalSeconds;
                await SaveWalletTransation(wt);
            }
        }

        public async Task<bool> EndGame(int gameId, long roundId)
        {
            if (!enableEndGame)
                return false;
            try
            {
                var user = userSession.User;

                var url = String.Format("{0}/wallet/endgame", proxyUrl);
                var prm = String.Format("operator={0}&cust_id={1}&game_id={2}&round_id={3}&extra_info={4}",
                    user.Operator.Tag,
                    user.ExternalId,
                    gameId,
                    roundId,
                    extraInfo);

                //var doc = HttpUtil.GetResponseXml(url, prm);
                var doc = await GetXmlAsync(url, prm);
                var root = doc.Element("resp");
                if (root == null) return false;

                var elem = root.Element("error_code");
                if (elem == null) return false;

                return Convert.ToInt32(elem.Value) == 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public async Task<Result<decimal, ErrorCode>> GetBalance(int gameId, int platform)
        {
            var user = userSession.User;

            var url = string.Format("{0}/wallet/getbalance", proxyUrl);
            var prm = string.Format("operator={0}&cust_id={1}&currency={2}&game_id={3}&platform={4}&extra_info={5}",
                user.Operator.Tag,
                user.ExternalId,
                user.Currency.IsoCode,
                gameId,
                platform,
                extraInfo);

            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["SessionKey"] = userSession.SessionKey,
                ["UserId"] = userSession.UserId,
                ["GameId"] = gameId,
                ["Operator"] = user.Operator.Tag
            }))
            {
                try
                {
                    var doc = await GetXmlAsync(url, prm);

                    var root = doc.Element("resp");
                    if (root == null)
                    {
                        logger.LogWarning("The response xml format not matched !");
                        logger.LogWarning(doc.ToString());
                        return ErrorCode.InternalError;
                    }

                    var errorCode = root.Element("error_code");
                    if (errorCode != null && int.Parse(errorCode.Value) != 0)
                    {
                        logger.LogWarning("Got error {0} during call get balance api", errorCode.Value);
                        logger.LogWarning(doc.ToString());
                        return ErrorCode.InternalError;
                    }

                    var balance = root.Element("balance");
                    if (balance == null)
                    {
                        logger.LogWarning("The balance element not existed !");
                        logger.LogWarning(doc.ToString());
                        return ErrorCode.InternalError;
                    }
                    return Convert.ToDecimal(balance.Value);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Request: {url}?{prm}");
                    logger.LogError(ex, ex.Message);
                }
                return ErrorCode.InternalError;
            }
        }

        public async Task<bool> NotifyTrxId(string trx_id, string ref_trx_id)
        {
            try
            {
                var user = userSession.User;

                var url = String.Format("{0}/wallet/updateinfo", proxyUrl);
                var prm = String.Format("operator={0}&trx_id={1}&ref_trx_id={2}",
                    user.Operator.Tag,
                    trx_id,
                    ref_trx_id);

                //var doc = HttpUtil.GetResponseXml(url, prm);
                var doc = await GetXmlAsync(url, prm);
                var root = doc.Element("resp");
                if (root == null) return false;

                var elem = root.Element("error_code");
                if (elem == null) return false;

                return Convert.ToInt32(elem.Value) == 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return false;
            }
        }

        private async Task<XDocument> GetXmlAsync(string url, string prm)
        {
            var httpClient = httpClientFactory.CreateClient();
            return await httpClient.GetXmlAsync($"{url}?{prm}");
        }
    }
}
