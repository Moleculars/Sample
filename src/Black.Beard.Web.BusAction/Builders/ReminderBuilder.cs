using Bb.ActionBus;
using Bb.Brokers;
using Bb.BusAction.Services;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Reminder;
using Bb.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Bb.BusAction.Builders
{

    [DependOf(typeof(BusActionBuilder))]
    public class ReminderBuilder : IBuilder
    {

        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {

            //services.AddSingleton(typeof(EngineGeneratorConfiguration), typeof(EngineGeneratorConfiguration));

            // Initialisation of DbFactory (it is specfic for type 'ReminderStoreSqlServer')
            DbProviderFactories.RegisterFactory(ActionBusConstants.SqlproviderInvariantName, SqlClientFactory.Instance);

            services.AddSingleton(typeof(IReminderStore), typeof(Services.ReminderStore));
            services.AddSingleton(typeof(IReminderRequest), typeof(ReminderService));
            services.AddSingleton(typeof(ActionRunner<ActionBusContext>), typeof(ActionRunner<ActionBusContext>));
            services.AddSingleton(typeof(IReminderResponseService), typeof(ReminderResponseService));

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var services = app.ApplicationServices;

            ReminderService reminder = services.GetService(typeof(IReminderRequest)) as ReminderService;
            IReminderResponseService responseReminder = services.GetService(typeof(IReminderResponseService)) as IReminderResponseService;

            reminder.AddResponses(responseReminder);

        }

    }


}
