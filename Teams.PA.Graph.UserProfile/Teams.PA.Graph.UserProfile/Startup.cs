using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using Teams.PA.Graph.UserProfile.Models;
using Teams.PA.Graph.UserProfile.Services;

namespace Teams.PA.Graph.UserProfile
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.Configure<AzureAdOptions>(Configuration.GetSection("AzureAd"));
            services.AddScoped<ITokenAcquisitionService, TokenAcquisitionService>();
            services.AddScoped<IGraphService, GraphService>();           
            services.AddMemoryCache();
            services.AddSession();
            services
               .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(o =>
               {
                   AzureAdOptions authSettings = Configuration.GetSection("AzureAd").Get<AzureAdOptions>();

                   //Identify the identity provider
                   o.Authority = authSettings.Authority;

                   //Require tokens be saved in the AuthenticationProperties on the request
                   //We need the token later to get another token
                   o.SaveToken = true;

                   o.TokenValidationParameters = new TokenValidationParameters
                   {
                       //Both the client id and app id URI of this API should be valid audiences
                       ValidAudiences = new List<string> { authSettings.ClientId, authSettings.ApplicationIdUri }
                   };
               });



            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
