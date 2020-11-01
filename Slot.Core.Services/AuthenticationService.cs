using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Slot.Core.Data;
using Slot.Core.Extensions;
using Slot.Core.Security;
using Slot.Core.Services.Abstractions;
using Slot.Core.Services.Models;
using Slot.Model;
using Slot.Model.Entity;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slot.Core.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDistributedCache cache;
        private readonly CachedSettings cachedSettings;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<AuthenticationService> logger;

        public AuthenticationService(IDistributedCache cache,
                                     CachedSettings cachedSettings,
                                     IHttpClientFactory httpClientFactory,
                                     ILogger<AuthenticationService> logger)
        {
            this.cache = cache;
            this.cachedSettings = cachedSettings;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<Result<AuthenticateResult, ErrorCode>> Authenticate(string merchant, string token)
        {
            try
            {
                if (!cachedSettings.OperatorsByName.TryGetValue(merchant, out Operator op))
                {
                    return ErrorCode.WrongParameter;
                }
                var encodedToken = op.EncodeToken ? WebUtility.UrlEncode(token) : token;
                var authenticateUri = string.Format("{0}/authenticate/authenticate", GetWalletProxyUrl());
                var authenticateParam = string.Format("operator={0}&token={1}", op.Tag, encodedToken);
                var httpClient = httpClientFactory.CreateClient();
                var rsp = await httpClient.GetStringAsync($"{authenticateUri}?{authenticateParam}");
                var doc = XDocument.Parse(rsp);
                var root = doc.Element("resp");
                if (root == null)
                {
                    logger.LogWarning($"Called auth api of {authenticateUri} got null");
                }
                if (!Int32.TryParse(root.Element("error_code").Value, out int errorCode) || errorCode != 0)
                {
                    logger.LogWarning($"Got error for auth, error code: {root.Element("error_code")}");
                    return ErrorCode.SessionExpired;
                }
                var currencyCode = ConvertToIsoCurrencyCode(root.Element("currency_code").Value);
                var memberId = root.Element("cust_id").Value;
                var memberName = root.Element("cust_name").Value;
                //var sessionKey = SecurityTokenProvider.Create(); // root.Element("session_id").Value;
                var sessionKey = root.Element("session_id").Value;
                var testCust = root.Element("test_cust").Value.Trim();
                var isTestAccount = testCust == "1" || testCust.ToLower() == "true";
                var extraInfo = root.Element("extra_info") != null ? root.Element("extra_info").Value : string.Empty;
                var result = new AuthenticateResult
                {
                    SessionKey = op.EncodeToken ? WebUtility.UrlEncode(sessionKey) : sessionKey,
                    Currency = currencyCode,
                    ExtraInfo = extraInfo,
                    IsTestAccount = isTestAccount,
                    MemberId = memberId,
                    MemberName = memberName
                };
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return ErrorCode.SessionExpired;
        }

        private string GetWalletProxyUrl()
        {
            var proxyurl = string.Empty;
            var setting = cache.Get<Data.Models.Setting>("URL_WALLETPROXY");
            if (setting != null) proxyurl = setting.Value;
            if (!string.IsNullOrEmpty(proxyurl)) return proxyurl;

            if (cachedSettings.ConfigSettings.TryGetValue("URL_WALLETPROXY", out ConfigurationSetting configurationSetting))
                proxyurl = configurationSetting.Value;
            return proxyurl;
        }

        private string ConvertToIsoCurrencyCode(string currencyCode)
        {
            switch (currencyCode)
            {
                case "RMB": return "CNY";
                case "UUS": return "GPI";
            }
            return currencyCode;
        }

    }
}
