//using Microsoft.Graph;
using Teams.PA.Graph.UserProfile.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace Teams.PA.Graph.UserProfile.Services
{
    public interface IGraphService    
    {               
        Task<UserInfo> GetUserProfileAsync(string accessToken);
    }
}
