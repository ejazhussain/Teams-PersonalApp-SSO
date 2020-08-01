using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.PA.Graph.UserProfile.Services
{
    public interface ITokenAcquisitionService  
    {
        Task<string> GetOnBehalfAccessTokenAsync(string graphScope, string jwtToken);
    }

  
}
