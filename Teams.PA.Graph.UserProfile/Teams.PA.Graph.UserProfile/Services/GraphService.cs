using Teams.PA.Graph.UserProfile.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Teams.PA.Graph.UserProfile.Services
{
    public class GraphService : IGraphService
    {
        private readonly string serviceEndpoint = "https://graph.microsoft.com/v1.0";

        public async Task<UserInfo> GetUserProfileAsync(string accessToken)
        

            UserInfo profile = new UserInfo();
            try
            {

                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                // Endpoint for my profile
                Uri usersEndpoint = new Uri(serviceEndpoint + "/me");

                HttpResponseMessage response = await client.GetAsync(usersEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    JObject jResult = JObject.Parse(responseContent);
                    profile.DisplayName = jResult["displayName"].ToString();
                    profile.JobTitle = jResult["jobTitle"].ToString();
                    profile.Email = jResult["mail"].ToString();
                    profile.OfficeLocation = jResult["officeLocation"].ToString();
                    profile.MobilePhone = jResult["mobilePhone"].ToString();
                }

                else
                {
                    return null;
                }

            }

            catch (Exception e)
            {
                return null;
            }

            return profile;
        }


    }
}
