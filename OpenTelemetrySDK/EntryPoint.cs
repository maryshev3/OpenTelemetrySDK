using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using System.Diagnostics;
using Npgsql;

namespace OpenTelemetrySDK
{
    public class EntryPoint
    {
        public static void InjectOpenTelemetryJager(IServiceCollection serviceCollection, IConfiguration configuration, string applicationName, string[] activitySources)
        {
            serviceCollection
                .AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    ActivitySource.AddActivityListener(new ActivityListener()
                    {
                        ShouldListenTo = _ => true,
                        Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                    });
                    
                    builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName));
                    builder.SetSampler(new AlwaysOnSampler());
                    builder.AddHttpClientInstrumentation();
                    builder.AddSource(activitySources);
                    builder.AddNpgsql();
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddJaegerExporter();
                    builder.AddOtlpExporter(options => options.Endpoint = new Uri(configuration.GetValue<string>("OtlpExporterOptions:Endpoint") ?? string.Empty));
                    builder.ConfigureServices(services =>
                    {
                        services.Configure<JaegerExporterOptions>(configuration.GetSection("Jaeger"));
                    });
                });
        }
    }
}
