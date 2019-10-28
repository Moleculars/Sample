using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bb.Middleware
{

    public class LoggingCatchMiddleware
    {

        public LoggingCatchMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(new { Message = "http query failed", Exception = ex }, TraceLevel.Error.ToString());
                throw;
            }
        }

        private readonly RequestDelegate _next;

    }

}
