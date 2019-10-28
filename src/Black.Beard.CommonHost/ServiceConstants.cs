namespace Bb.CommonHost
{

    public static class ServiceConstants
    {

        public static string[] AssemblyDocumentations =
        {
             "Black.Beard*.xml"
        };

        public static string Version = "1.0.0";
        public static string VersionUmber = "v" + Version.Split('.')[0];

        public static string SawggerName = "My First Swagger Environment";
        public static string LicenceName = "Only usable with a valid partner contract.";
        public static string Title = "Black Beard Action APIs";

        public static string ApiKey = "ApiKey";
        public static string Key = "key";
        public static string UseSwagger = "useSwagger";
        public static string Namespace = "Namespace";

    }

}




//services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddJwtBearer(options => {
//            options.TokenValidationParameters = 
//                 new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,

//                ValidIssuer = "Fiver.Security.Bearer",
//                ValidAudience = "Fiver.Security.Bearer",
//                IssuerSigningKey = Bb.Security.Jwt.JwtSecurityKey.Create("fiversecret ")
//            };

//            options.Events = new JwtBearerEvents
//            {
//                OnAuthenticationFailed = context =>
//                {
//                    Console.WriteLine("OnAuthenticationFailed: " + 
//                        context.Exception.Message);
//                    return Task.CompletedTask;
//                },
//                OnTokenValidated = context =>
//                {
//                    Console.WriteLine("OnTokenValidated: " + 
//                        context.SecurityToken);
//                    return Task.CompletedTask;
//                }
//            };

//        });
