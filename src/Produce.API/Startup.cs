using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ProduceAPI
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            // Set default scheme to `Bearer`
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    // set the Identity.API service as the authority on authentication/authorization
                    options.Authority = "http://localhost:5000";
                    options.RequireHttpsMetadata = false;

                    // set the name of the API that's talking to the Identity API
                    options.Audience = "ProduceAPI";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}