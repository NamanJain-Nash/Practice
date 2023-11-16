using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace HttpFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<ICosmoDbService, CosmoDbService>();
            builder.Services.AddTransient<IStoargeQueueService, StoargeQueueService>();
            builder.Services.AddLogging();
        }
    }
}
