using System.Collections.Generic;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ragnar.AuthorizationServer
{
    public class Startup
    {
        #region Public Properties

        public IConfiguration Configuration { get; }

        #endregion

        #region Private Properties

        private static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        private static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("read-cars", "Read Cars"),
                new ApiScope("read-trucks", "Read Trucks")
            };

        private static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>
            {
                new ApiResource("rest-api", "REST API")
                {
                    Scopes = new string[] { "read-cars", "read-trucks" }
                }
            };

        private static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "one-client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "read-cars", "read-trucks" }
                }
            };

        #endregion

        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion

        #region Public Methods

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddIdentityServer()
                .AddInMemoryIdentityResources(IdentityResources)
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryApiResources(ApiResources)
                .AddInMemoryClients(Clients);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        #endregion
    }
}
