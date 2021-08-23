using FS.Infrastructure.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Infrastructure.Service
{
    public class ExibitorWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ExibitorLinks _exibitorLinks;
        private readonly IConfiguration _configuration;

        public ExibitorWorker(IServiceScopeFactory serviceScopeFactory,
            ExibitorLinks exibitorLinks, IConfiguration configuration)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._exibitorLinks = exibitorLinks;
            this._configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(15));
            Log.Information("Start Exibitor Worker.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var exibitorUrl = _exibitorLinks.Links.Take();

                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var httpClient = scope.ServiceProvider
                            .GetRequiredService<IHttpClientFactory>().CreateClient("Fs");

                        var data = await httpClient.GetStringAsync(exibitorUrl);

                        if (!string.IsNullOrEmpty(data))
                        {
                            // Create exibitor Add to DB
                        }

                        await Task.Delay(TimeSpan.FromSeconds(4));
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Exibitor Worker error");
                }
            }
        }
    }
}