using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Http;



namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private static HttpClient httpClient = new HttpClient();

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = "4e88e948-ca13-427f-87ff-da668543d6df";

            var telemetryClient = new TelemetryClient(configuration);

            String[] adresses = { "https://bing.com", "https://opsconfig.com", "https://microsoft.com", "https://google.com", "https://xbox.com" };

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                using (telemetryClient.StartOperation<RequestTelemetry>("workeroperation"))
                {



                    {
                        foreach (String add in adresses)
                        {
                            // var res = httpClient.GetAsync("https://bing.com").Result.StatusCode;
                            var res = httpClient.GetAsync(add).Result.StatusCode;
                            _logger.LogInformation($"{add} -->:" + res);
                        }
                    }


                }

                telemetryClient.TrackEvent("PrimeSoft Training Events");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
