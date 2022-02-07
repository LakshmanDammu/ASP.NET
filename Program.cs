using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;

namespace WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<ITelemetryInitializer, MyCustomTelemetryInitializer >();
                    services.AddApplicationInsightsTelemetryProcessor<MyCustomTelemetryProcessor>();
                    services.ConfigureTelemetryModule<QuickPulseTelemetryModule>((mod, opt) => mod.AuthenticationApiKey = "4e88e948-ca13-427f-87ff-da668543d6df");

                    services.AddApplicationInsightsTelemetryWorkerService();
                });

        internal class MyCustomTelemetryInitializer : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                (telemetry as ISupportProperties).Properties["PrimeSoftDemoKey"] = "PrimeSoftDemoValue";
            }
        }

        internal class MyCustomTelemetryProcessor : ITelemetryProcessor
        {
            ITelemetryProcessor next;

            public MyCustomTelemetryProcessor(ITelemetryProcessor next)
            {
                this.next = next;
            }

            public void Process(ITelemetry item)
            {
                this.next.Process(item);
            }
        }

    }
}
