using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Bb;
using Bb.Builders;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Middleware;
using Bb.Security.Jwt;
using Bb.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;

namespace Bb.CommonHost
{
    public class BaseStartup
    {

        public BaseStartup(IConfiguration configuration)
        {
            _builderlist = new List<IBuilder>();
            Configuration = configuration;
            _useSwagger = configuration.GetValue<bool>(ServiceConstants.UseSwagger);
        }

        public IConfiguration Configuration { get; }
        public static ExposedTypes Types { get; private set; }

        private readonly bool _useSwagger;
        private readonly List<IBuilder> _builderlist;

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {

            //Func<ServiceDescriptor, string> GetText = (s) => {
            //    if (s.ImplementationType != null)
            //        return s.ImplementationType.FullName;
            //    else if(s.ImplementationInstance != null)
            //        return s.ImplementationInstance.GetType().FullName;
            //    return string.Empty;
            //};
            //foreach (var item in services)
            //    Debug.WriteLine($"{item.ServiceType.ToString()} => {GetText(item)}");

            // var vars = Environment.GetEnvironmentVariables();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            BaseStartup.Types = services.RegisterConfigurations(Configuration);
            ResolveBuilders(services);

            if (_useSwagger)
                AddSwagger(services);


            services.AddControllers();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                ;

            //services.AddRazorPages();
            //services.AddServerSideBlazor();

        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(swagger =>
            {
                swagger.DescribeAllEnumsAsStrings();
                swagger.DescribeAllParametersInCamelCase();
                swagger.IgnoreObsoleteActions();
                swagger.AddSecurityDefinition(ServiceConstants.Key, new Swashbuckle.AspNetCore.Swagger.ApiKeyScheme { Name = ServiceConstants.ApiKey });

                //swagger.TagActionsBy(a => a.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor b
                //    ? b.ControllerTypeInfo.Assembly.FullName.Split('.')[2].Split(',')[0].Replace("Web", "")
                //    : a.ActionDescriptor.DisplayName
                //);

                //swagger.DocInclusionPredicate((f, a) =>
                //{
                //    return a.ActionDescriptor is ControllerActionDescriptor b && b.MethodInfo.GetCustomAttributes<ExternalApiRouteAttribute>().Any();
                //});

                swagger.SwaggerDoc(ServiceConstants.VersionUmber, new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = ServiceConstants.Title,
                    License = new License() { Name = ServiceConstants.LicenceName },
                    Version = ServiceConstants.Version,
                });

                var doc = DocumentationHelpers.ConcateDocumentations(ServiceConstants.AssemblyDocumentations);
                if (doc != null)
                    swagger.IncludeXmlComments(() => doc);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            else
            {
                app.UseHsts();
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthorization();

            string Namespace = Configuration.GetValue<string>(ServiceConstants.Namespace);
            if (!string.IsNullOrEmpty(Namespace))
            {

                // Load static content from module assemblies
                var staticProviders = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .AsParallel()
                    .Where(a => a.FullName.StartsWith(Namespace) && !a.FullName.Contains("Tests,"))
                    .Select(a => new EmbeddedFileProvider(a))
                    .ToList<IFileProvider>();

                staticProviders.Add(new PhysicalFileProvider(Path.Join(env.ContentRootPath, "wwwroot")));

            }

            app
                .UseMiddleware<LoggingCatchMiddleware>()
                .UseMiddleware<LoggingSupervisionMiddleware>()
                .UseMiddleware<ReadTokenMiddleware>(app.ApplicationServices.GetService(typeof(JwtTokenConfiguration)))
                ;

            foreach (var item in _builderlist)
                item.Configure(app, env);


            if (_useSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    string path = @"/" + Path.Combine("swagger", ServiceConstants.VersionUmber, "swagger.json").Replace("\\", "/");
                    c.SwaggerEndpoint(path, ServiceConstants.SawggerName);
                    c.DefaultModelsExpandDepth(0);
                });

            }

            app.UseEndpoints(endpoints =>
            {

                //endpoints.MapBlazorHub();
                //endpoints.MapFallbackToPage("/_Host");

                try
                {
                    Trace.WriteLine($"Addings controllers", TraceLevel.Info.ToString());
                    endpoints.MapControllers();
                    //var routes = endpoints.ServiceProvider.GetService(typeof(IActionDescriptorCollectionProvider));                
                }
                catch (Exception e)
                {
                    Trace.WriteLine($"Failed to add Controllers. {e.Message}", TraceLevel.Error.ToString());
                    throw;
                }
            });



        }


        protected virtual void ResolveBuilders(IServiceCollection services)
        {

            // Sort by dependencies
            List<Type> _types = new List<Type>();
            foreach (var item in Initialization.ExposedServices)
                PlaceWithDependances(_types, item);

            foreach (var item in _types)
                _builderlist.Add((IBuilder)Activator.CreateInstance(item));

            foreach (var item in _builderlist)
                item.Initialize(services, Configuration);

        }

        private int PlaceWithDependances(List<Type> types, Type type)
        {

            if (!types.Contains(type))
            {

                var p = type.GetCustomAttributes(typeof(DependOfAttribute), true)
                    .OfType<DependOfAttribute>()
                    .Select(c => c.Type)
                    .ToList();

                int index = 0;

                foreach (var item in p)
                {
                    var i2 = PlaceWithDependances(types, item);
                    if (i2 > index)
                        index = i2 + 1;
                }

                types.Insert(index, type);

                return index;

            }

            return -1;

        }

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
