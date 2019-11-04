using Bb.Brokers;
using Bb.Configurations;
using System.ComponentModel;

namespace MolecularClient.Commands
{
    public class ConfigurationBroker
    {

        public Brokers Brokers { get; set; }

        public IFactoryBroker ApplyConfiguration(IFactoryBroker broker)
        {

            if (Brokers != null)
            {

                if (Brokers.Servers != null)
                    foreach (var server in Brokers.Servers)
                        broker.Add(server);

                if (Brokers.Publishers != null)
                    foreach (var publisher in Brokers.Publishers)
                        broker.Add(publisher);

                if (Brokers.Subscribers != null)
                    foreach (var subscriber in Brokers.Subscribers)
                        broker.Add(subscriber);

            }

            return broker;

        }

    }

    public class Brokers
    {

        [Description("List of broker server")]
        public ServerBrokerConfiguration[] Servers { get; set; }

        [Description("List of broker publisher")]
        public BrokerPublishParameter[] Publishers { get; set; }

        [Description("List of broker subsribers")]
        public BrokerSubscriptionParameter[] Subscribers { get; set; }

    }

}