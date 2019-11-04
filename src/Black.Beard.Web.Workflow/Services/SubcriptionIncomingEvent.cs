using Bb.Brokers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Bb.Workflows.Services
{

    public class SubcriptionIncomingEvent : SubscriptionInstance, IHostedService
    {

        public SubcriptionIncomingEvent(IFactoryBroker brokers, EngineGeneratorConfiguration configuration, WebEngineProvider provider, KilledGracefulInterceptor killed)
            : base("Service consume incoming event.")
        {
            _provider = provider;
            _domainHeader = configuration.EngineGeneratorModel.DomainHeader;
            Subscription = brokers.CreateSubscription(configuration.EngineGeneratorModel.WorkflowQueue, context => Callback(context));
            _deadQueue = brokers.CreatePublisher(configuration.EngineGeneratorModel.PublishDeadQueueWorkflow);
            _killed = killed;
            _killed.StopAction = this.PrepareStop;
        }

        private Task Callback(IBrokerContext context)
        {

            if (context.Headers.TryGetValue(_domainHeader, out object header))
            {

                var domain = System.Text.Encoding.ASCII.GetString((byte[])header);
                var workflowEngine = _provider.Get(domain);

                try
                {

                    workflowEngine.EvaluateEvent(context.Utf8Data);

                    context.Commit();

                }
                catch (System.Exception exception)
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
                                RepushedAt = WorkflowClock.Now(),
                                OriginExchange = context.Exchange,
                                OriginRoutingKey = context.RoutingKey,
                                context.Utf8Data,
                                Exception = exception,
                            },
                            _headers
                        );

                        context.Commit();

                    }


                }

                return Task.CompletedTask;

            }

            throw new System.Exception($"missing domain header {_domainHeader}");

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

        WebEngineProvider _provider;
        private readonly string _domainHeader;

        public IBrokerPublisher _deadQueue { get; internal set; }

        private void PrepareStop()
        {

            Subscription.Dispose();

        }

        private readonly KilledGracefulInterceptor _killed;
    
    }

}
