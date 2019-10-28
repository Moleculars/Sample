using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Bb.Security.Jwt
{

    public class JwtTokenManager
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenManager"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public JwtTokenManager(JwtTokenConfiguration configuration)
        {


            this.expiryInMinutes = configuration.ExpiryInMinutes;

            bool f = true;

            if (string.IsNullOrEmpty(configuration.SecurityKey))
            {
                Trace.WriteLine($"{nameof(configuration.SecurityKey)}", TraceLevel.Error.ToString());
                f = true;
            }

            if (string.IsNullOrEmpty(configuration.Audience))
            {
                Trace.WriteLine($"{nameof(configuration.Audience)}", TraceLevel.Error.ToString());
                f = true;
            }

            if (string.IsNullOrEmpty(configuration.Issuer))
            {
                Trace.WriteLine($"{nameof(configuration.Issuer)}", TraceLevel.Error.ToString());
                f = true;
            }

            if (f)
                _validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = configuration.Audience,
                    ValidIssuer = configuration.Issuer,
                    IssuerSigningKey = JwtSecurityKey.Create(configuration.SecurityKey),
                    ValidateAudience = configuration.ValidateAudience,
                    ValidateIssuer = configuration.ValidateIssuer,
                    ValidateIssuerSigningKey = configuration.validateIssuerSigningKey,
                };

        }


        public ClaimsPrincipal ValidToken(string payload)
        {

            if (_validationParameters != null)
            {
                JwtSecurityTokenHandler handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var user = handler.ValidateToken(payload, _validationParameters, out SecurityToken validatedToken);
                return user;
            }

            return null;

        }

        public JwtTokenManager AddSubject(string subject)
        {
            this.subject = subject;
            return this;
        }

        public JwtTokenManager AddMail(string mail)
        {
            this.mail = mail;
            return this;
        }

        public JwtTokenManager AddPseudo(string pseudo)
        {
            this.pseudo = pseudo;
            return this;
        }

        public JwtTokenManager AddClaim(string type, string value)
        {
            claims.Add(type, value);
            return this;
        }

        public JwtTokenManager AddClaims(Dictionary<string, string> claims)
        {
            this.claims.Union(claims);
            return this;
        }

        public JwtTokenManager AddExpiry(int expiryInMinutes)
        {
            this.expiryInMinutes = expiryInMinutes;
            return this;
        }

        public string Build()
        {

            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            EnsureArguments();

            var claims = new List<Claim>
            {
              new Claim(JwtRegisteredClaimNames.Email, mail),
              new Claim(JwtRegisteredClaimNames.UniqueName, subject),
              new Claim(JwtRegisteredClaimNames.Sub, pseudo),
              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }
            .Union(this.claims.Select(item => new Claim(item.Key, item.Value)));

            var token = new JwtSecurityToken
            (
                issuer: _validationParameters.ValidIssuer,
                audience: _validationParameters.ValidAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: new SigningCredentials(_validationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        #region private

        private void EnsureArguments()
        {

            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException("Subject");

            if (string.IsNullOrEmpty(_validationParameters.ValidIssuer))
                throw new ArgumentNullException("Issuer");

            if (string.IsNullOrEmpty(_validationParameters.ValidAudience))
                throw new ArgumentNullException("Audience");
        }

        private readonly TokenValidationParameters _validationParameters;
        private string subject = "";
        private Dictionary<string, string> claims = new Dictionary<string, string>();
        private int expiryInMinutes = 5;
        private string mail;
        private string pseudo;

        #endregion
    }
}
