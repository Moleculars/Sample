using Bb.Brokers;
using Bb.Web;
using Bb.Workflows.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bb.Workflows.Builders
{

    public class WorkflowBuilder : IBuilder
    {


        public WorkflowBuilder()
        {

        }


        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(WebEngineProvider), typeof(WebEngineProvider));
            services.AddSingleton(typeof(EngineGeneratorConfiguration), typeof(EngineGeneratorConfiguration));
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }


    }


}
