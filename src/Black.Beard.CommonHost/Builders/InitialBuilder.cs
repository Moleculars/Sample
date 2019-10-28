using Bb.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bb.Builders
{

    public class InitialBuilder : IBuilder
    {

        public InitialBuilder()
        {

        }


        public void Initialize(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(KilledGracefulInterceptor), typeof(KilledGracefulInterceptor));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {



        }


    }


}
