using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Teams.PA.Graph.UserProfile.Models
{
    /// <summary>
    /// AzureAdOptions class contain value application configuration properties for Azure Active Directory.
    /// </summary>
    public class AzureAdOptions
    {
        /// <summary>
        /// Gets or sets Azure Ad Instance
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Gets or sets Client Id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets Client secret.
        /// </summary>
        public string ClientSecret { get; set; }


        /// <summary>
        /// Gets or sets tenant Id.
        /// </summary>
        public string TenantId { get; set; }
               

        /// <summary>
        /// Gets or sets Application Id URI.
        /// </summary>
        public string ApplicationIdUri { get; set; }

        /// <summary>
        /// Gets or sets Graph API scope.
        /// </summary>
        public string GraphScope { get; set; }


        /// <summary>
        /// Get Authority
        /// </summary>
        public string Authority => Instance + TenantId;
    }
}

