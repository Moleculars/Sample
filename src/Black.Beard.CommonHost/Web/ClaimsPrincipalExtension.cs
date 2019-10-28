using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bb.Web
{

    public static class ClaimsPrincipalExtension
    {


        public static bool CanAccess(this ClaimsPrincipal self, ModelStateDictionary modelState, string emailOwner)
        {

            return true;

        }


        public static string Email(this ClaimsPrincipal self)
        {

            return self.FindFirst(c => c.Type == "email")?.Value ?? null;

        }


        //public static System.IdentityModel.Tokens.Jwt.JwtSecurityToken GetFromJwt(string token)
        //{
        //    var jwtHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        //    var jwt = jwtHandler.ReadJwtToken(token);
        //    // return jwt.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToArray();
        //    return jwt;
        //}


    }

}
