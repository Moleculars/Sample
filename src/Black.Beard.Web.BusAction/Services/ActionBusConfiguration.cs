using Bb.Brokers;

namespace Bb.BusAction.Services
{

    public class ActionBusConfiguration
    {

        public ActionBusConfiguration(ActionBusBrokerConfiguration actionBusBrokerConfiguration, IFactoryBroker brokerConfiguration)
        {
            this.ActionBusBrokerConfiguration = actionBusBrokerConfiguration;
            this.Broker = brokerConfiguration;
        }

        public ActionBusBrokerConfiguration ActionBusBrokerConfiguration { get; private set; }

        public IFactoryBroker Broker { get; private set; }

    }

}