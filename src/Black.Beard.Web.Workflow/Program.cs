using Bb.CommonHost;
using Microsoft.AspNetCore.Hosting;

namespace Bb.Workflows
{

    public class Program : ProgramService<Startup>
    {

        public Program() 
            : base()
        {

        }

        public static void Main(string[] args)
        {

            var service = new Program()
                .Run(args);

        }

        public override IWebHostBuilder CreateServiceHostBuilder(string[] args)
        {
            return base.CreateServiceHostBuilder(args);
        }

    }






}
