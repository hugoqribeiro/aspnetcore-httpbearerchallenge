using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ragnar.HttpBearerChallenge;

namespace Ragnar.RestApi
{
    public partial class Startup
    {
        #region Public Properties

        public IConfiguration Configuration { get; }

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
            services
                .AddControllersWithViews();

            services
                .AddAuthentication("Bearer")
                .AddJwtBearer(
                    (options) =>
                    {
                        options.Authority = "https://localhost:5001";
                        options.Audience = "rest-api";
                        options.RequireHttpsMetadata = false;
                        options.IncludeErrorDetails = true;
                        options.RefreshOnIssuerKeyNotFound = true;
                        options.SaveToken = true;

                        // NOTE:
                        // This builds the WWW-Authenticate header

                        options.Events = new HttpBearerChallengeEvents();
                    });

            services
                .AddAuthorization(
                    (options) =>
                    {
                        options.AddPolicy(
                            "read-cars",
                            (policy) =>
                            {
                                // NOTE:
                                // This makes the scope required by the endpoints
                                // available for the HttpBearerChallengeBuilder

                                policy.RequireScope("read-cars");
                            });
                        options.AddPolicy(
                            "read-trucks",
                            (policy) =>
                            {
                                // NOTE:
                                // This makes the scope required by the endpoints
                                // available for the HttpBearerChallengeBuilder

                                policy.RequireScope("read-trucks");
                            });
                    });
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        #endregion
    }
}
