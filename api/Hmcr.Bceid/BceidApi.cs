using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using BceidService;
using Hmcr.Model;
using Hmcr.Model.Utils;

namespace Hmcr.Bceid
{
    public interface IBceidApi
    {
        Task<(string error, BceidAccount account)> GetBceidAccountAsync(string username, string userType);
        Task<(string error, BceidAccount account)> GetBceidAccountCachedAsync(string username, string userType);
    }

    public class BceidApi : IBceidApi
    {
        private readonly BCeIDServiceSoapClient _client;
        private readonly Dictionary<string, BceidAccount> _accountCache; //no need for ConcurrentDictionary
        private readonly System.Timers.Timer _timer;
        private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public BceidApi(BCeIDServiceSoapClient client)
        {
            _client = client;
            _accountCache = new Dictionary<string, BceidAccount>();
            _timer = new System.Timers.Timer();
            _timer.Elapsed += new ElapsedEventHandler(RefreshCache);
            _timer.Interval = _client.CacheLifespan * 60000; //minutes
            _timer.Enabled = true;
        }

        private void RefreshCache(object source, ElapsedEventArgs e)
        {
            Debug.WriteLine($"BCeID Cache clean up: {_accountCache.Keys.Count} entries.");
            _accountCache.Clear();
        }

        public async Task<(string error, BceidAccount account)> GetBceidAccountCachedAsync(string username, string userType)
        {
            //to minimize the BCeID web service calls - may have a performance issue when multiple fresh users log in at the same time.            
            await _semaphore.WaitAsync(); 

            try
            {
                var key = username + "||" + userType;
                if (_accountCache.ContainsKey(key))
                {
                    Debug.WriteLine($"BCeID cache hit: {key}");
                    return ("", _accountCache[key]);
                }

                var (error, account) = await GetBceidAccountAsync(username, userType);

                if (account != null)
                {
                    Debug.WriteLine($"BCeID new key: {key}");
                    _accountCache[key] = account;
                }

                return (error, account);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<(string error, BceidAccount account)> GetBceidAccountAsync(string username, string userType)
        {
            var typeCode = userType.IsIdirUser() ? BCeIDAccountTypeCode.Internal : BCeIDAccountTypeCode.Business;

            var request = new AccountDetailRequest();
            request.requesterAccountTypeCode = BCeIDAccountTypeCode.Internal;
            request.requesterUserGuid = _client.Guid;
            request.accountTypeCode = typeCode;
            request.userId = username;
            request.onlineServiceId = _client.Osid;

            var response = await _client.getAccountDetailAsync(request);

            if (response.code != ResponseCode.Success)
            {
                return (response.message, null);
            }
            else if (response.failureCode == FailureCode.NoResults)
            {
                return ("", null);
            }

            var account = new BceidAccount();

            account.Username = response.account.userId.value;
            account.UserGuid = new Guid(response.account.guid.value);
            account.UserType = userType;

            if (account.UserType.IsBusinessUser())
            {
                account.BusinessGuid = new Guid(response.account.business.guid.value);
                account.BusinessLegalName = response.account.business.legalName.value;
                account.BusinessNumber = Convert.ToDecimal(response.account.business.businessNumber.value ?? "0");
                account.DoingBusinessAs = response.account.business.doingBusinessAs.value.IsEmpty() ? account.BusinessLegalName : response.account.business.doingBusinessAs.value;
            }

            account.DisplayName = response.account.displayName.value;
            account.FirstName = response.account.individualIdentity.name.firstname.value;
            account.LastName = response.account.individualIdentity.name.surname.value;
            account.Email = response.account.contact.email.value;

            return ("", account);
        }
    }
}
