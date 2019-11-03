using Bb.CommonHost;
using Microsoft.AspNetCore.Hosting;

namespace Bb.BusAction
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
            var a = base.CreateServiceHostBuilder(args);
            return a;
        }

    }






}
