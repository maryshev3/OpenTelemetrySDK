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
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Configuration;

namespace OpenTelemetrySDK
{
    public class EntryPoint
    {
        public void InjectOpenTelemetryJager(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection
                .AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder.AddAspNetCoreInstrumentation();
                    builder.AddJaegerExporter();
                    builder.ConfigureServices(services => 
                    {
                        services.Configure<JaegerExporterOptions>(configuration.GetSection("Jaeger"));
                    });
                });
        }
    }
}
