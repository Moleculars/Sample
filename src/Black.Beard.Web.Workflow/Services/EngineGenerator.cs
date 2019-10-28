using Bb.Brokers;
using Bb.ComponentModel;
using Bb.Workflows.Converters;
using Bb.Workflows.Models;
using Bb.Workflows.Models.Configurations;
using Bb.Workflows.Outputs;
using Bb.Workflows.Outputs.Mom;
using Bb.Workflows.Parser;
using Bb.Workflows.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Bb.Workflows.Services
{


    public class EngineGenerator<TContext>
        where TContext : RunContext, new()
    {

        public EngineGenerator(EngineGeneratorConfiguration configuration)
        {

            this._configuration = configuration;

            this._methods = new Dictionary<string, MethodInfo>();
            this.serializer = new JsonWorkflowSerializer();

            this._templateTypes = new Type[] { typeof(TemplatesProviders) };
            this._metadataTypes = new Type[] { typeof(MatadataProviders) };

        }

        public EngineGenerator<TContext> SetPath(string path)
        {
            this._path = path;
            AddFolderToDiscovery(_path);
            
            return this;
        
        }

        public WorkflowEngine CreateEngine()
        {
            return new WorkflowEngine()
            {
                Serializer = this.serializer,
                Processor = CreateWorkflowProcessor(),
            };
        }

        private WorkflowProcessor CreateWorkflowProcessor()
        {

            DirectoryInfo d = new DirectoryInfo(_path);

            var configurations = GetConfigurations(d, "workflow");

            var template = new TemplateRepository(_templateTypes)
            {
                DefaultAction = TemplateModels.DefaultAction,
            };

            var metadatas = new MetadatRepository(_metadataTypes)
            {
                DefaultAction = MetadataModels.DefaultAction.ToDictionary(c => c.Key, c => c.Value),
            };

            WorkflowProcessor processor = new WorkflowProcessor<TContext>(configurations, null)
            {
                LoadExistingWorkflowsByExternalId = this.LoadExistingWorkflowsByExternalId,
                OutputActions = CreateOutput,
                Templates = template,
                Metadatas = metadatas,
            };

            return processor;

        }

        public List<Workflow> LoadExistingWorkflowsByExternalId(string key)
        {

            //(key) => storage.GetBy<Workflow, string>(key, c => c.ExternalId).ToList()*;

            return new List<Workflow>();
        }



        public OutputAction CreateOutput()
        {

            return new SetPropertiesOutputAction(
                new PushBusActionOutputAction(
                    //new PushBusActionOutputActionInMemory(storage,
                    //    new PushModelOutputActionInMemory(storage)
                    //)
                )
                {
                    Brokers = this._configuration.BrokerConfiguration.GetFactory(),
                    PublisherName = this._configuration.EngineGeneratorModel.ActionBusPublisher,
                }
            );

        }

        #region business rules

        private void ResolveMethods()
        {

            this._methods.Clear();

            var methods = TypeDiscovery.Instance.GetMethods(BindingFlags.Public | BindingFlags.Static, typeof(bool), null)
                .Where(c => EvalMethod(c))
                .ToList()
                ;

            foreach (var method in methods)
                if (method.GetCustomAttribute(typeof(System.ComponentModel.DisplayNameAttribute)) is System.ComponentModel.DisplayNameAttribute attribute)
                {
                    if (this._methods.ContainsKey(attribute.DisplayName))
                        Trace.WriteLine($"a method named {attribute.DisplayName} in {method.ToString() } already exists. It is ignored");
                    else
                        this._methods.Add(attribute.DisplayName, method);
                }

        }

        private void AddFolderToDiscovery(string path)
        {

            DirectoryInfo d = new DirectoryInfo(path);
            if (!d.Exists)
                throw new DirectoryNotFoundException(path);

            d.Refresh();

            TypeDiscovery.Instance.AddDirectories(d);
            TypeDiscovery.Instance.AddDirectories(d.GetDirectories());

            TypeDiscovery.Instance.LoadAssembliesFromFolders();

            ResolveMethods();

        }

        private bool EvalMethod(MethodInfo method)
        {

            var parameters = method.GetParameters();

            if (parameters.Length == 0)
                return false;

            if (parameters[0].ParameterType != typeof(TContext))
                return false;

            return true;

        }

        #endregion business rules

        #region configurations document

        private WorkflowsConfig GetConfigurations(DirectoryInfo d, string extension)
        {

            if (!extension.StartsWith("*."))
            {
                if (extension.StartsWith("."))
                    extension = "*" + extension;
                else
                    extension = "*." + extension;
            }

            WorkflowsConfig configs = new WorkflowsConfig();

            Func<WorkflowConfigVisitor> f = () =>
            {

                WorkflowConfigVisitor visitor = new WorkflowConfigVisitor();

                foreach (var method in this._methods)
                    visitor.AddRule(method.Key, method.Value);

                return visitor;

            };

            var files = d.GetFiles(extension, SearchOption.AllDirectories).ToList();
            HashSet<string> _paths = new HashSet<string>();
            foreach (var item in files)
                if (_paths.Add(item.Directory.FullName))
                    WorkflowsConfigLoader.Load(configs, f, item.Directory.FullName, extension, null, null);

            return configs;

        }

        #endregion configurations document



        private IWorkflowSerializer serializer;
        private string _path;
        private readonly Type[] _templateTypes;
        private readonly Type[] _metadataTypes;
        private readonly EngineGeneratorConfiguration _configuration;
        private readonly Dictionary<string, MethodInfo> _methods;
    }


}
