namespace Bb.Security.Jwt
{
    public class JwtTokenConfiguration
    {


        public bool AllApisAreSecurized { get; set; }

        public string SecurityKey { get; set; }

        public string Issuer { get; set; }

        public string Audience { get; set; }



        public bool ValidateAudience { get; set; } = true;

        public bool ValidateIssuer { get; set; } = true;

        public bool validateIssuerSigningKey { get; set; } = true;

        public int ExpiryInMinutes { get; set; } = 10;

    }
}
