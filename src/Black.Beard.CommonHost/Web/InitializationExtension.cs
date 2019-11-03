using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.ComponentModel.Exceptions;
using Bb.ComponentModel.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bb.Web
{
    public static class InitializationExtension
    {

        /// <summary>
        /// Loads the directories json files from configuration.
        /// </summary>
        /// <param name="configurationBuilder">The configuration builder.</param>
        /// <param name="self">The self.</param>
        /// <returns></returns>
        public static IConfigurationBuilder LoadDirectoriesJsonFiles(this IConfigurationBuilder configurationBuilder, Initialization self)
        {

            if (self.ConfigurationPaths != null)
            {

                bool configLoaded = false;

                foreach (var configuration in self.ConfigurationPaths)
                {

                    var dir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, configuration));
                    if (dir.Exists)
                        foreach (var file in dir.GetFiles("*.json"))
                            if (EvaluateJson(file))
                            {
                                configLoaded = true;
                                Trace.WriteLine($"add configuration file '{file.FullName}'", TraceLevel.Info.ToString());
                                configurationBuilder.AddJsonFile(file.FullName, optional: false, reloadOnChange: false);
                            }

                }

                if (!configLoaded)
                    Trace.WriteLine("no configuration file loaded", TraceLevel.Info.ToString());

            }
            else
                Trace.WriteLine("no configuration folder specified", TraceLevel.Info.ToString());

            return configurationBuilder;

        }

        private static bool EvaluateJson(FileInfo file)
        {

            if (file.Length == 0)
            {
                Trace.WriteLine($"configuration file '{file.FullName}' is empty", TraceLevel.Info.ToString());
                return false;
            }

            var txt = File.ReadAllText(file.FullName).Trim();
            if (string.IsNullOrEmpty(txt))
            {
                Trace.WriteLine($"configuration file '{file.FullName}' is empty", TraceLevel.Info.ToString());
                return false;
            }

            try
            {
                Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(file.FullName));
            }
            catch (Exception e)
            {
                Trace.WriteLine($"configuration file '{file.FullName}' is failed ({e.Message})", TraceLevel.Error.ToString());
                return false;
            }

            return true;

        }

        /// <summary>
        /// Register all configurations from class decorated by. ExposeClass(Context = "Configuration", DisplayName = "key in configuration")
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static ExposedTypes RegisterConfigurations(this IServiceCollection services, IConfiguration configuration)
        {

            var types = new ExposedTypes();

            // Add configuration type manualy. 
            var __configuration = typeof(ExposedTypeConfigurations).LoadConfiguration(configuration) as ExposedTypeConfigurations;
            if (__configuration != null)
                types.Add(__configuration)
                     .AddAttributesInTypeDescriptors();


            // Add configuration type by attribute in code. 
            types.Remove(typeof(ExposedTypeConfigurations));
            var configs = types.GetTypes(ConstantsCore.Configuration).ToArray();


            RegenerateSchemas(types);


            foreach (var _type in configs)
                foreach (var attribute in _type.Value)
                    AppendConfiguration(services, configuration, _type.Key, attribute);

            return types;

        }

        private static void RegenerateSchemas(ExposedTypes types)
        {
            // 
            var dir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Schemas"));
            if (!dir.Exists)
                dir.Create();
            var configs = types.GetTypes(ConstantsCore.Configuration).ToArray();
            foreach (var _type in configs)
            {
                if (_type.Value.Count == 1)
                {
                    var name = _type.Value.First().Name;
                    WriteSchema(dir, _type.Key, name);
                }
            }

            WriteSchema(dir, typeof(Initialization), "appsettings");

        }

        private static void WriteSchema(DirectoryInfo dir, Type type, string filename)
        {
            var schema = SchemaHelper.GenerateSchemaForClass(type, filename);
            string filePath = Path.Combine(dir.FullName, filename + ".json");
            File.WriteAllText(filePath, schema.ToString());
            Trace.WriteLine($"validation schema {type.Name} has generated in the file {filename}", TraceLevel.Info.ToString());
        }

        private static void AppendConfiguration(IServiceCollection services, IConfiguration configuration, Type type, ExposeClassAttribute attribute)
        {

            Func<IServiceProvider, object> func = null;
            var typeExposed = attribute.ExposedType ?? type;
            string name = attribute.Name;

            if (!typeExposed.IsAssignableFrom(type))
                throw new IocException($"Try to register {type.FullName} configuration with contract {typeExposed.FullName}. {typeExposed.Name} can't be assignated from {type.Name}");

            var factory = new Factory<object>(type, new Type[] { });

            func = srvs =>
            {
                var _configuration = factory.Create();
                configuration.Bind(name, _configuration);
                return _configuration;
            };

            if (attribute.LifeCycle == IocScopeEnum.Singleton)
                services.Add(ServiceDescriptor.Singleton(typeExposed, func));
            else
                services.Add(ServiceDescriptor.Transient(typeExposed, func));

            Trace.WriteLine($"{name} configuration node mapped on {type} and added in {attribute.LifeCycle}");

        }

        //private static IEnumerable<Type> FindIConfigureOptions(Type type)
        //{

        //    var serviceTypes = type.GetTypeInfo().ImplementedInterfaces
        //        .Where(t => t.GetTypeInfo().IsGenericType &&
        //        (t.GetGenericTypeDefinition() == typeof(IConfigureOptions<>)
        //        || t.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>)));

        //    return serviceTypes;

        //}

        public static object LoadConfiguration(this Type self, IConfiguration configuration)
        {

            var arr = TypeDescriptor.GetAttributes(self).OfType<ExposeClassAttribute>().ToArray();

            if (arr.Length > 0)
            {
                string name = arr[0].Name;
                object _configuration = Activator.CreateInstance(self, new object[] { });
                configuration.Bind(name, _configuration);
                return _configuration;
            }

            return null;

        }

    }

}
