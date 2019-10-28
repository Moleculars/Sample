using Bb.CommonHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bb.Workflows
{

    public class Startup : BaseStartup
    {

        public Startup(IConfiguration configuration)
            : base(configuration)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            //services.AddHostedService<>

        }        

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
        }


    }
    
}
