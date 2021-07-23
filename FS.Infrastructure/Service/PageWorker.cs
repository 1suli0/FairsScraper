using FS.Infrastructure.Data;
using FS.Infrastructure.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FS.Infrastructure.Service
{
    public class PageWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ExibitorLinks _exibitorLinks;

        public PageWorker(IServiceScopeFactory serviceScopeFactory,
            ExibitorLinks exibitorLinks)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._exibitorLinks = exibitorLinks;
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

                        var main = Utility.CreateNode(data);

                        var content = Utility.GetMainContent(main);

                        var numberOfPages = int.Parse(Utility.GetNumberOfPages(content));

                        for (int i = 0; i < numberOfPages; i++)
                        {
                            var url = $"/exhibitors-and-products/exhibitor-index" +
                                $"/exhibitor-index-anuga/?fw_goto=aussteller/" +
                                $"blaettern&fw_ajax=1&paginatevalues=&start={i * 20}";

                            var response = await httpClient.GetAsync(url);

                            var pageData = await response.Content.ReadAsStringAsync();

                            var page = Utility.CreateNode(pageData);

                            try
                            {
                                var urls = Utility.GetUrls(page).Distinct().ToList();

                                Log.Information($"Page: {i}");
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, $"INCOMING DATA: {pageData}");
                                Debugger.Break();
                            }

                            await Task.Delay(1500);
                        }
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