using System;
using System.Diagnostics;
using System.Reflection;
using Bb.ComponentModel;
using Bb.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bb.CommonHost
{

    public class ProgramService<TStartup>
        where TStartup : class
    {

        public static ProgramService<TStartup> Instance { get; private set; }

        public ProgramService()
        {
            Instance = this;
        }

        public ProgramService<TStartup> Run(string[] args)
        {

            using (Bb.Logs.Serilog.SerilogTraceListener.Initialize())
            {

                var serviceHostBuilder = CreateServiceHostBuilder(args)
                    .UseStartup<TStartup>()
                    ;

                Run(serviceHostBuilder);

            }

            return this;

        }

        private static void Run(IWebHostBuilder serviceHostBuilder)
        {

            using (var serviceHost = serviceHostBuilder.Build())
            {

                IConfiguration configuration = (IConfiguration)serviceHost.Services.GetService(typeof(IConfiguration));
                Globals.SetCulture(configuration.GetValue<string>("Culture"));
                Globals.SetFormatDateCulture(configuration.GetValue<string>("FormatDateCulture"));

                var taskRun = serviceHost.RunAsync();

                // Wait exit
                Console.CancelKeyPress += (sender, eventArgs) =>
                {

                    Trace.WriteLine("Received a stop notification, engine shutdown");

                    KilledGracefulInterceptor.Stop();

                    Trace.WriteLine("engine main service has stopped.");
                    eventArgs.Cancel = true;

                };

                taskRun.Wait();

                var taskStop = serviceHost.StopAsync();

                taskStop.Wait();

            }

        }

        public virtual IWebHostBuilder CreateServiceHostBuilder(string[] args)
        {
            // Load assemblies
            var _config = Initialization.Load();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {

                    var env = context.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                          .LoadDirectoriesJsonFiles(_config)
                          .AddEnvironmentVariables()
                          .AddCommandLine(args)
                    ;

                    if (env.IsDevelopment())
                    {

                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                            config.AddUserSecrets(appAssembly, optional: true);

                    }

                })
                .ConfigureLogging((hostingContext, logging) =>
                {

                    var env = hostingContext.HostingEnvironment;
                    var configLogging = hostingContext.Configuration.GetSection("Logging");
                    logging.AddConfiguration(configLogging);
                    if (env.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                    }

                })
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                });

            ;

        }


    }






}
