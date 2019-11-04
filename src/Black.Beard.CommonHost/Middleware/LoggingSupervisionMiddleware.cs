using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bb.Middleware
{
    public class LoggingSupervisionMiddleware
    {

        public LoggingSupervisionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            Stopwatch sw = new Stopwatch();

            sw.Start();
            await _next(context);
            sw.Stop();

            Trace.WriteLine(new { Message = "http query", sw.Elapsed }, TraceLevel.Error.ToString());

        }

        private readonly RequestDelegate _next;
    }

}
