using Bb.Security.Jwt;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bb.Middleware
{
    public class ReadTokenMiddleware
    {

        public ReadTokenMiddleware(RequestDelegate next, JwtTokenConfiguration tokenConfiguration)
        {
            _next = next;

            this.Securized = tokenConfiguration.AllApisAreSecurized;
            if (this.Securized)
            {
                try
                {
                    this._jwtTokenManager = new JwtTokenManager(tokenConfiguration);
                }
                catch (System.Exception e)
                {
                    Trace.WriteLine(e.Message);
                    throw;
                }
            }

        }

        public async Task Invoke(HttpContext context)
        {
            if (this.Securized)
            {
                if (context.Request.Headers.ContainsKey("authorization"))
                {

                    if (this._jwtTokenManager == null)
                        throw new NullReferenceException("JwtTokenManager");

                    var tokenText = context.Request.Headers["authorization"];
                    context.User = this._jwtTokenManager.ValidToken(tokenText);
                }
                else
                    throw new Exception("not auhentication. please add a header 'authorization' with a valid token JWT");
            }

            await _next(context);

        }

        private readonly RequestDelegate _next;

        public bool Securized { get; }

        private readonly JwtTokenManager _jwtTokenManager;

    }

}
