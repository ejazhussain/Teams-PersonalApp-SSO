using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Teams.PA.Graph.UserProfile.Models;

namespace Teams.PA.Graph.UserProfile.Services
{
    public class TokenAcquisitionService : ITokenAcquisitionService
    {
        private readonly IOptions<AzureAdOptions> azureAdOptions;

        private readonly string[] scopesRequestedByMsalNet = new string[]
        {
            "openid",
            "profile",
            "offline_access",
        };

        public TokenAcquisitionService(IOptions<AzureAdOptions> azureAdOptions)
        {
            this.azureAdOptions = azureAdOptions;
        }
        public async Task<string> GetOnBehalfAccessTokenAsync(string graphScopes, string jwtToken)
        {
            if (jwtToken == null)
            {
                throw new ArgumentNullException(jwtToken, "tokenValidationContext.SecurityToken should be a JWT Token");
            }

            UserAssertion userAssertion = new UserAssertion(jwtToken, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            IEnumerable<string> requestedScopes = graphScopes.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();

            var confidentialClientApp = ConfidentialClientApplicationBuilder.Create(azureAdOptions.Value.ClientId)
                 .WithAuthority(azureAdOptions.Value.Authority)
                 .WithClientSecret(azureAdOptions.Value.ClientSecret)
                 .Build();
            
            var result = await confidentialClientApp.AcquireTokenOnBehalfOf(
                requestedScopes.Except(scopesRequestedByMsalNet),
                userAssertion)
                .ExecuteAsync();
            return result.AccessToken;
        }
    }
}
