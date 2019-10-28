using System;
using System.IdentityModel.Tokens.Jwt;

namespace Bb.Security.Jwt
{
    public sealed class JwtToken
    {


        public JwtToken(string token)
        {
            this.token = new JwtSecurityToken( token);
        }


        public string Value => new JwtSecurityTokenHandler().WriteToken(this.token);


        private JwtSecurityToken token;


    }
}
