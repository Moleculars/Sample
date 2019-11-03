using Bb.Brokers;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;


namespace Bb.Workflows.Services
{

    public class SubcriptionIncomingAdapter : SubscriptionInstance, IHostedService
    {

        public SubcriptionIncomingAdapter(IFactoryBroker brokers, EngineGeneratorConfiguration configuration, WebEngineProvider provider)
            : base("Service consume incoming bus action broker queue")
        {
            _provider = provider;
            _domainHeader = configuration.EngineGeneratorModel.DomainHeader;
            _CountFailedHeader = configuration.EngineGeneratorModel.CountFailedHeader;
            _maxRetry = configuration.EngineGeneratorModel.MaxRetry;
            Subscription = brokers.CreateSubscription(configuration.EngineGeneratorModel.WorkflowQueue, context => Callback(context));
            _deadQueue = brokers.CreatePublisher(configuration.EngineGeneratorModel.PublishDeadQueueWorkflow);
        }

        private Task Callback(IBrokerContext context)
        {

            object header;

            if (context.Headers.TryGetValue(_domainHeader, out header))
            {

                var workflowEngine = _provider.Get(header.ToString());

                try
                {

                    workflowEngine.EvaluateEvent(context.Utf8Data);

                }
                catch (System.Exception exception)
                {

                    int count = 0;
                    if (context.Headers.TryGetValue(_CountFailedHeader, out header))
                        count = (int)header;

                    if (context.CanBeRequeued() && count < _maxRetry)
                    {

                        // if can be requeued and retry count < retry max

                        count++;
                        if (context.Headers.ContainsKey(_CountFailedHeader))
                            context.Headers[_CountFailedHeader] = count;
                        else
                            context.Headers.Add(_CountFailedHeader, count);

                        context.RequeueLast();

                    }
                    else
                    {

                        // push in deadqueue
                        _deadQueue.Publish(
                            new
                            {
                                //Order = order,
                                //order.ExecutedAt,
                                //order.PushedAt,
                                exception
                            }
                        );

                        //context.Reject();

                        throw;
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
        private readonly string _CountFailedHeader;
        private readonly int _maxRetry;

        public IBrokerPublisher _deadQueue { get; internal set; }

    }

}
