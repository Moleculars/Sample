using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Bb.Web
{

    public interface IBuilder
    {

        void Initialize(IServiceCollection services, IConfiguration configuration);

        void Configure(IApplicationBuilder app, IWebHostEnvironment env);

    }
}
