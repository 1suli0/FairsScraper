﻿using FS.Infrastructure.Context;
using FS.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FS.Infrastructure.Extensions
{
    public static class Startup
    {
        public static void ConfigureDBContext(this IServiceCollection services,
           IConfiguration configuration)
        {
            services.AddDbContextPool<DBContext>(opts =>
            {
                opts.UseMySql(configuration.GetConnectionString("sqlConnection"),
                    o =>
                    {
                        o.MigrationsAssembly("FS.Infrastructure");
                        o.EnableRetryOnFailure();
                    });
            });
        }

        public static void ConfigureHttpClient(this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddHttpClient("Fs", client =>
            {
                client.BaseAddress = configuration.GetValue<Uri>("Website");
            });
        }

        public static void ConfigureHostedService(this IServiceCollection services)
        {
            services.AddHostedService<Worker>();
        }
    }
}