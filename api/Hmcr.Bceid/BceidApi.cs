using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BceidService;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;

namespace Hmcr.Bceid
{
    public interface IBceidApi
    {
        Task<(string Error, BceidAccount account)> GetBceidAccountAsync(string requestUserGuid, string username, string userType);
    }

    public class BceidApi : IBceidApi
    {
        private readonly BCeIDServiceSoapClient _client;

        public BceidApi(BCeIDServiceSoapClient client)
        {
            _client = client;
        }

        public async Task<(string Error, BceidAccount account)> GetBceidAccountAsync(string requestUserGuid, string username, string userType)
        {
            var typeCode = userType.IsIdirUser() ? BCeIDAccountTypeCode.Internal : BCeIDAccountTypeCode.Business;

            var request = new AccountDetailRequest();
            request.requesterAccountTypeCode = BCeIDAccountTypeCode.Internal;
            request.requesterUserGuid = requestUserGuid;
            request.accountTypeCode = BCeIDAccountTypeCode.Business;
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

            if (!account.UserType.IsBusinessUser())
            {
                account.BusinessGuid = new Guid(response.account.business.guid.value);
                account.BusinessLegalName = response.account.business.legalName.value;
                account.BusinessNumber = Convert.ToDecimal(response.account.business.businessNumber.value);
                account.DisplayName = response.account.business.doingBusinessAs.value;
            }
            else
            {
                account.DisplayName = response.account.displayName.value;
            }

            account.FirstName = response.account.individualIdentity.name.firstname.value;
            account.LastName = response.account.individualIdentity.name.surname.value;
            account.Email = response.account.contact.email.value;           

            return ("", account);
        }

    }
}
