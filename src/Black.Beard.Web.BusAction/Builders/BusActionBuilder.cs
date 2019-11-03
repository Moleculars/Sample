using Bb.ActionBus;
using Bb.Brokers;
using Bb.BusAction.Services;
using Bb.Reminder;
using Bb.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bb.BusAction.Builders
{

    public class BusActionBuilder : IBuilder
    {

        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton(typeof(IFactoryBroker), typeof(RabbitFactoryBrokers));
            services.AddSingleton(typeof(ActionBusConfiguration), typeof(ActionBusConfiguration));

            services.CreateActionRepositories<ActionBusContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.ApplicationServices.RegisterBusinessActions<ActionBusContext>();
            var broker = app.ApplicationServices.GetService<IFactoryBroker>();
            var brokerConfiguration = app.ApplicationServices.GetService<BrokerConfiguration>();
            brokerConfiguration.ApplyConfiguration(broker);

        }


    }

}
