using FS.Infrastructure.Data;
using FS.Infrastructure.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
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
        private readonly IConfiguration _configuration;

        public PageWorker(IServiceScopeFactory serviceScopeFactory,
            ExibitorLinks exibitorLinks, IConfiguration configuration)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._exibitorLinks = exibitorLinks;
            this._configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            Log.Information("Start Page Worker.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var httpClient = scope.ServiceProvider
                            .GetRequiredService<IHttpClientFactory>().CreateClient("Fs");

                        var data = await httpClient.GetStringAsync(_configuration["Website:ExibitorsUrl"]);

                        var main = Utility.CreateNode(data);

                        var content = Utility.GetMainContent(main);

                        var numberOfPages = int.Parse(Utility.GetNumberOfPages(content));

                        for (int i = 0; i < numberOfPages; i++)
                        {
                            var url = $"{_configuration["Website:ExibitorsUrl"]}" +
                                $"{_configuration["Website:PaginationUrl"]}" +
                                $"{i * 20}";

                            var response = await httpClient.GetAsync(url);

                            var pageData = await response.Content.ReadAsStringAsync();

                            var page = Utility.CreateNode(pageData);

                            var urls = Utility.GetUrls(page).Distinct().ToList();

                            foreach (var u in urls)
                                _exibitorLinks.Links.Add(u);

                            await Task.Delay(TimeSpan.FromSeconds(2));
                        }
                    }

                    await Task.Delay(TimeSpan.FromHours(8));
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Page Worker error");
                }
            }
        }
    }
}