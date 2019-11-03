using Bb.Brokers;
using System;

namespace Bb.Workflows.Services
{

    public class EngineGeneratorConfiguration
    {

        public EngineGeneratorConfiguration(EngineGeneratorModel engineGeneratorModel, IFactoryBroker broker, EngineConfigurationModel engineConfiguration, IServiceProvider services)
        {
            this.EngineGeneratorModel = engineGeneratorModel;
            this.Broker = broker;
            this.EngineConfiguration = engineConfiguration;
            this.Services = services;
        }

        public EngineGeneratorModel EngineGeneratorModel { get; set; }

        public IFactoryBroker Broker { get; set; }

        public EngineConfigurationModel EngineConfiguration { get; set; }

        public IServiceProvider Services { get; }

    }


}

