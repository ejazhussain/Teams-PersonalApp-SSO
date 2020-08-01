using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Teams.PA.Graph.UserProfile.Models;
using Teams.PA.Graph.UserProfile.Services;

namespace Teams.PA.Graph.UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class BaseController : ControllerBase
    {
        private readonly IOptions<AzureAdOptions> azureAdOptions;
        private readonly ILogger logger;
        private readonly ITokenAcquisitionService tokenAcquisitionService;

        public BaseController(IOptions<AzureAdOptions> azureAdOptions,
            ILogger logger, ITokenAcquisitionService tokenAcquisitionService)
        {
            this.azureAdOptions = azureAdOptions;
            this.logger = logger;
            this.tokenAcquisitionService = tokenAcquisitionService;
        }

        /// <summary>
        /// Gets user's Azure AD object id.
        /// </summary>
        public string UserObjectId
        {
            get
            {
                var oidClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";
                var claim = this.User.Claims.First(p => oidClaimType.Equals(p.Type, StringComparison.Ordinal));
                
                return claim.Value;
            }
        }

        /// <summary>
        /// Get user Azure AD access token.
        /// </summary>
        /// <returns>Token to access MS graph.</returns>
        public async Task<string> GetAccessTokenAsync()
        {
            
            try
            {                
             
                var jwtToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"].ToString()).Parameter;

                var accesstoken = await tokenAcquisitionService.GetOnBehalfAccessTokenAsync(azureAdOptions.Value.GraphScope, jwtToken);
                
                return accesstoken;

            }           
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in fetching token : {ex.Message}.");
                throw;
            }

        }

        public async Task<string> GetAccessTokenAsyncWithCaching()
        {
            List<string> scopeList = azureAdOptions.Value.GraphScope.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {

                // Gets user account from the accounts available in token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.getaccountasync?view=azure-dotnet
                // Concatenation of UserObjectId and TenantId separated by a dot is used as unique identifier for getting user account.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.accountid.identifier?view=azure-dotnet#Microsoft_Identity_Client_AccountId_Identifier

                var confidentialClientApp = ConfidentialClientApplicationBuilder.Create(azureAdOptions.Value.ClientId)
                   .WithAuthority(azureAdOptions.Value.Authority)
                   .WithClientSecret(azureAdOptions.Value.ClientSecret)
                   .Build();

                var account = await confidentialClientApp.GetAccountAsync($"{UserObjectId}.{azureAdOptions.Value.TenantId}");
                //var accounts = await confidentialClientApp.GetAccountsAsync();


                // Attempts to acquire an access token for the account from the user token cache.
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.identity.client.clientapplicationbase.acquiretokensilent?view=azure-dotnet
                AuthenticationResult result = await confidentialClientApp
                    .AcquireTokenSilent(scopeList, account)
                    .ExecuteAsync();
                return result.AccessToken;

            }
            catch (MsalUiRequiredException msalex)
            {
                // Getting new token using AddTokenToCacheFromJwtAsync as AcquireTokenSilent failed to load token from cache.
                //TokenAcquisitionService tokenAcquisitionHelper = new TokenAcquisitionHelper(this.confidentialClientApp);
                try
                {
                    this.logger.LogInformation($"MSAL exception occurred while trying to acquire new token. MSAL exception details are found {msalex}.");
                    var jwtToken = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"].ToString()).Parameter;
                    var accesstoken = await tokenAcquisitionService.GetOnBehalfAccessTokenAsync(azureAdOptions.Value.GraphScope, jwtToken);
                    //return await tokenAcquisitionHelper.AddTokenToCacheFromJwtAsync(azureAdOptions.Value.GraphScope, jwtToken);
                    return accesstoken;
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, $"An error occurred in GetAccessTokenAsync: {ex.Message}.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in fetching token : {ex.Message}.");
                throw;
            }

        }

    }
}
