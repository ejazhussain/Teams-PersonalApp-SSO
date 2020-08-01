using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Teams.PA.Graph.UserProfile.Models;
using Teams.PA.Graph.UserProfile.Services;

namespace Teams.PA.Graph.UserProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController 
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        
        private readonly ILogger<UserController> logger;
        
        private readonly IGraphService graphService;

        public UserController(IOptions<AzureAdOptions> azureAdOptions,
            ILogger<UserController> logger,
            ITokenAcquisitionService tokenAcquisitionService,
            IGraphService graphService) : base(azureAdOptions, logger, tokenAcquisitionService)
        {
        
            this.logger = logger;
            this.graphService = graphService;
        }

        [HttpGet]
        [Route("GetUserProfile")]
        public async Task<IActionResult> GetUserProfileAsync()
        {

            try
            {

                //var rng = new Random();
                //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                //{
                //    Date = DateTime.Now.AddDays(index),
                //    TemperatureC = rng.Next(-20, 55),
                //    Summary = Summaries[rng.Next(Summaries.Length)]
                //})
                //.ToArray();


                string accessToken = await this.GetAccessTokenAsync();
                var result = await graphService.GetUserProfileAsync(accessToken);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"An error occurred in GetUserProfile: {ex.Message}");
                throw;
            }           
           
        }
    }
}
