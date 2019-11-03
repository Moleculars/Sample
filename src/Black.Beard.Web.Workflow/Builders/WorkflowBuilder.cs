using Bb.Brokers;
using Bb.Dao;
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

            services.AddTransient(typeof(SqlManager), typeof(SqlManager));
            services.AddSingleton(typeof(IFactoryBroker), typeof(RabbitFactoryBrokers));
            services.AddSingleton(typeof(WebEngineProvider), typeof(WebEngineProvider));
            services.AddSingleton(typeof(EngineGeneratorConfiguration), typeof(EngineGeneratorConfiguration));

            services.AddSingleton(typeof(SubcriptionIncomingAdapter), typeof(SubcriptionIncomingAdapter));
        
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var broker = app.ApplicationServices.GetService<IFactoryBroker>();
            var brokerConfiguration = app.ApplicationServices.GetService<Services.Broker>();
            brokerConfiguration.ApplyConfiguration(broker);

        }


    }


}
