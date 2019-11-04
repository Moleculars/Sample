using Bb.Brokers;
using Bb.Option;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MolecularClient.Commands
{

    // workflow http https://localhost:5001 Test3 "F:\Src\Moleculars\Sample\src\Tests\test1.txt"
    // workflow broker brokers.json PublishToWorkflow Test "F:\Src\Moleculars\Sample\src\Tests\test1.txt"

    public static partial class Command
    {

        public static CommandLineApplication CommandPushworkflow(this CommandLineApplication app)
        {

            var cmd = app.Command("workflow", config =>
            {

                config.Description = "push a message on workflow server";
                config.HelpOption(HelpFlag);

            });

            var cmd2 = cmd.Command("http", config =>
            {

                config.Description = "push a message on workflow server using http";
                config.HelpOption(HelpFlag);

                var validator = new GroupArgument(config, false);

                var serverName = validator.Argument("server",
                    "Url server of the workflow server. (this argument is required)"
                    , ValidatorExtension.EvaluateRequired
                    );

                var domain = validator.Argument("domain",
                    "target domain workflow (this argument is required)"
                    , ValidatorExtension.EvaluateRequired
                    );

                var content = validator.Argument("content",
                    "content that you want to push on the workflow server"
                    , ValidatorExtension.EvaluateRequired
                    );

                config.OnExecute(() =>
                {

                    if (validator.Evaluate() > 0)
                        return 2;

                    string model = GetContent(content.Value);

                    var client = new BbClientHttp(new Uri(serverName.Value));
                    var result = client.Post<RootResultModel<object>>($"api/workflowProcessor/Push/{domain.Value}", model /*, GetToken() */);

                    result.Wait();

                    Output.WriteLine("item successfully pushed on workflow server.");

                    Result = Helper.Parameters.ServerUrl;

                    return 0;

                });
            });

            var cmd3 = cmd.Command("broker", config =>
            {

                config.Description = "push a message on workflow server using http";
                config.HelpOption(HelpFlag);

                var validator = new GroupArgument(config, false);

                var configBroker = validator.Argument("config",
                    "broker configuration (this argument is required)"
                    , ValidatorExtension.EvaluateRequired
                    );

                var publisher = validator.Argument("publisher",
                    "server connection string (this argument is required)"
                    , ValidatorExtension.EvaluateRequired
                    );

                var domain = validator.Argument("domain",
                    "target domain workflow (this argument is required)"
                    , ValidatorExtension.EvaluateRequired
                    );

                var content = validator.Argument("content",
                    "content that you want to push on the workflow server"
                    , ValidatorExtension.EvaluateRequired
                    );

                config.OnExecute(() =>
                {

                    if (validator.Evaluate() > 0)
                        return 2;

                    string configRabbit = GetContent(configBroker.Value);
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationBroker>(configRabbit);

                    var publisherItem = result.Brokers.Publishers.First(c => c.Name == publisher.Value);

                    string model = GetContent(content.Value);

                    IFactoryBroker brokers = result.ApplyConfiguration(new RabbitFactoryBrokers())
                        .Initialize();

                    using (var publisherInstance = brokers.CreatePublisher(publisher.Value))
                    using (var trans = publisherInstance.BeginTransaction())
                    {
                        publisherInstance.Publish(publisherItem.DefaultRountingKey, model, new { domain = domain.Value });
                        publisherInstance.Commit();
                    }

                    Output.WriteLine("item successfully pushed on workflow server.");

                    return 0;

                });
            });

            return app;

        }

        private static string GetContent(string value)
        {

            string result = value;
            try
            {

                var file = string.Empty;
                
                if (File.Exists(value))
                    file = value;
                
                else if (File.Exists(Path.Combine(Environment.CurrentDirectory, value)))
                    file = Path.Combine(Environment.CurrentDirectory, value);

                if (File.Exists(file))
                    result = Helper.LoadContentFromFile(value);
            
            }
            catch (Exception)
            {

            }

            return result;

        }

    }

}