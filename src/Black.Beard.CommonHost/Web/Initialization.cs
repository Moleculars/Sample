using Bb.ComponentModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bb.Web
{

    public class Initialization
    {

        public static Initialization Load()
        {


            string path = Environment.CurrentDirectory;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            Initialization._configuration = new Initialization();
            configuration.Bind("Initialization", _configuration);


            HashSet<string> _types = new HashSet<string>();


            if (_configuration.Services != null)
                LoadAssemblies(path, _types);
            else
                Trace.WriteLine($"no specified assembly to load");


            var _builders = TypeDiscovery.Instance.GetTypes(c => typeof(IBuilder).IsAssignableFrom(c));
            foreach (var service in _builders)
                if (_types.Add(service.FullName))
                {
                    Trace.WriteLine($"builder '{service}' found and loaded", TraceLevel.Info.ToString());
                    ExposedServices.Add(service);
                }

            return _configuration;

        }

        private static void LoadAssemblies(string path, HashSet<string> _types)
        {


            foreach (InitializationAssemblyList service in _configuration.Services)
                foreach (var folder in service.Folders)
                {
                    folder.FolderPath = EnsureFolderExists(path, folder);
                    var countTypes = TypeDiscovery.Instance.LoadAssembliesFrom(folder.FolderPath);
                    Trace.WriteLine($"{countTypes} types loaded", TraceLevel.Error.ToString());
                }


            foreach (InitializationAssemblyList service in _configuration.Services)
                foreach (var folder in service.Folders)
                    foreach (var builder in folder.Builders)
                    {

                        var _builder = TypeDiscovery.Instance.ResolveByName(builder);
                        if (builder != null)
                        {
                            if (_types.Add(_builder.FullName))
                            {
                                Trace.WriteLine($"builder '{builder}' found and loaded", TraceLevel.Info.ToString());
                                ExposedServices.Add(_builder);
                            }
                        }
                        else
                            Trace.WriteLine($"builder '{builder}' not found", TraceLevel.Error.ToString());

                    }


        }

        public static List<Type> ExposedServices { get; } = new List<Type>();

        private static string EnsureFolderExists(string path, InitializationFolderAssembly folder)
        {

            var dir = folder.FolderPath;

            if (!Directory.Exists(dir))
            {

                dir = Path.Combine(path, dir);
                if (!Directory.Exists(dir))
                    throw new Bb.Exceptions.ConfigurationException($"Missing directory '{folder.FolderPath}' or '{dir}'");

                TypeDiscovery.Instance.AddDirectories(dir);

                Trace.WriteLine($"Add folder {dir} in type resolver", TraceLevel.Info.ToString());

            }

            return dir;

        }

        /// <summary>
        /// Gets or sets the configuration path's list.
        /// </summary>
        /// <value>
        /// The configuration paths.
        /// </value>

        public string[] ConfigurationPaths { get; set; }

        /// <summary>
        /// Gets or sets the assemblie's list must be load at start program.
        /// </summary>
        /// <value>
        /// The assemblies.
        /// </value>
        public List<InitializationAssemblyList> Services { get; set; }

        public static Initialization Instance => _configuration;

        private static Initialization _configuration;

    }

}
