using FS.Infrastructure.Context;
using FS.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Linq;

namespace FS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add
        // services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                     .WithMethods("GET", "POST", "PUT", "DELETE")
                     .AllowCredentials()
                     .SetIsOriginAllowed((host) => Configuration
                        .GetSection("AllowedHosts")
                        .Get<string[]>().Any());
                });
            });
            services.ConfigureDBContext(Configuration);
            services.ConfigureHttpClient(Configuration);
            services.ConfigureHostedService();
            services.ConfigureExibitorLinks();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure
        // the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            DBContext context)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error migrating database.");
                throw;
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}