using Bb.ActionBus;
using Bb.Brokers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Bb.BusAction.Services
{

    public class SubcriptionIncomingBusAction : SubscriptionInstance, IHostedService
    {

        public SubcriptionIncomingBusAction(IFactoryBroker brokers, ActionBusConfiguration configuration, ActionRunner<ActionBusContext> action)
            : base("Service consume incoming adapter broker queue")
        {
            _action = action;
            Subscription = brokers.CreateSubscription(configuration.ActionBusBrokerConfiguration.ActionBusQueue, context => Callback(context));
            _acknowledgeQueue = brokers.CreatePublisher(configuration.ActionBusBrokerConfiguration.AcknowledgeExchange);
            _deadQueue = brokers.CreatePublisher(configuration.ActionBusBrokerConfiguration.DeadQueueAction);
        }

        private Task Callback(IBrokerContext context)
        {

            var ctx = (ActionBusContext)_action.Evaluate(context.Utf8Data);
            if (ctx.Exception == null)
            {

                _acknowledgeQueue.Publish(
                    new
                    {
                        context.Utf8Data,
                        Exception = ctx.Exception
                    }
                );

                context.Commit();

            }
            else
            {

                if (context.CanBeRequeued())
                    context.RequeueLast();

                else
                {

                    Dictionary<string, object> _headers = new Dictionary<string, object>();
                    foreach (var item in context.Headers)
                        _headers.Add(item.Key, System.Text.Encoding.ASCII.GetString((byte[])item.Value));

                    _deadQueue.Publish(
                        new
                        {
                            RepushedAt = ClockActionBus.Now(),
                            OriginExchange = context.Exchange,
                            OriginRoutingKey = context.RoutingKey,
                            ctx.Exception
                        },
                        _headers
                    );

                    context.Commit();


                }

            }

            return Task.CompletedTask;

        }


        #region IHostedService

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        #endregion IHostedService

        private readonly ActionRunner<ActionBusContext> _action;

        public IBrokerPublisher _acknowledgeQueue { get; internal set; }

        public IBrokerPublisher _deadQueue { get; internal set; }

    }

}
