using FS.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Infrastructure.Service
{
    public class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var httpClient = scope.ServiceProvider
                            .GetRequiredService<IHttpClientFactory>().CreateClient("Fs");

                        var data = await httpClient.GetStringAsync("/exhibitors-and-products/exhibitor-index/exhibitor-index-anuga/");

                        var page = Utility.CreateNode(data);

                        var content = Utility.GetMainContent(page);

                        var pages = Utility.GetNumberOfPages(content);


                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Worker error");
                }
            }
        }
    }
}