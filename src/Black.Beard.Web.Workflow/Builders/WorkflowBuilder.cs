using Bb.Brokers;
using Bb.Dao;
using Bb.Web;
using Bb.Workflows.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

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

            services.AddSingleton(typeof(SubcriptionIncomingEvent), typeof(SubcriptionIncomingEvent));
            services.AddSingleton(typeof(SqlManagerConfiguration), typeof(SqlManagerConfiguration));


        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var broker = app.ApplicationServices.GetService<IFactoryBroker>();
            var brokerConfiguration = app.ApplicationServices.GetService<Services.Broker>();
            brokerConfiguration.ApplyConfiguration(broker);


            var modelStorage = app.ApplicationServices.GetService<SqlManagerConfiguration>();
            var modelStorageConfig = app.ApplicationServices.GetService<SqlServerManagerConfiguration>();

            modelStorage.ConnectionString = modelStorageConfig.ConnectionString;
            modelStorage.ProviderInvariantName = modelStorageConfig.ProviderInvariantName;

            DbProviderFactories.RegisterFactory(modelStorage.ProviderInvariantName, System.Data.SqlClient.SqlClientFactory.Instance);

        }


    }


}
