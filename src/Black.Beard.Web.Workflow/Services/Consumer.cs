
using Bb.Brokers;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bb.Workflows.Services
{

    public class Test
    {


        /*
         
            IFactoryBroker brokers = new RabbitFactoryBrokers()
                .AddServerFromConnectionString("Name=server1;Hostname=localhost;UserName=guest;Password=guest;Port=5672;UseLogger=true")
                .AddPublisherFromConnectionString("Name=publisher1;ServerName=server1;ExchangeType=DIRECT;ExchangeName=ech1;DefaultRountingKey=ech2")
                .AddSubscriberFromConnectionString("Name=subscriber37;ServerName=server1;ExchangeType=DIRECT;ExchangeName=ech1;StorageQueueName=ech2")
                .Initialize();

             */


        public Test(IFactoryBroker brokers)
        {
                       
            using (var subs = new SubscriptionInstances(brokers))
            {

                static Task callback(IBrokerContext ctx)
                {

                    //ctx.Commit();
                    //ctx.Reject();

                    return Task.CompletedTask;

                }

                // Add a subscriber
                var sub = subs.AddSubscription("sub1", "subscriber37", callback);

                //// push message in transaction
                //var publisher = brokers.CreatePublisher("publisher1");
                //using (publisher.BeginTransaction())
                //{
                //    publisher.Publish(new { uui = Guid.NewGuid() });
                //    publisher.Publish(new { uui = Guid.NewGuid() });
                //    publisher.Commit();
                //}

                DateTime d = DateTime.Now.AddMinutes(10);
                while (DateTime.Now < d)
                    Thread.Yield();

            }

        }

    }


    //    public interface IStreamSource<Toutput>
    //    {

    //        public Toutput Get();

    //    }


    //    public class Consumer<Tintput, TServiceTarget>
    //    {

    //        public Consumer(IStreamSource<Tintput> source, TServiceTarget service)
    //        {
    //            this._source = source;
    //            _service = service;
    //        }

    //        public void Run()
    //        {
    //            var message = _source.Get();
    //            Services.()
    //        }

    //        private readonly IStreamSource<Tintput> _source;
    //        readonly TServiceTarget _service;

    //    }


}


///*

//    // : IHostedService, IDisposable

//        #region IDisposable Support

//        private bool disposedValue = false; // To detect redundant calls

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!disposedValue)
//            {
//                if (disposing)
//                {
//                    // TODO: dispose managed state (managed objects).
//                }

//                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
//                // TODO: set large fields to null.

//                disposedValue = true;
//            }
//        }

//        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
//        // ~Consumer()
//        // {
//        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
//        //   Dispose(false);
//        // }

//        // This code added to correctly implement the disposable pattern.
//        public void Dispose()
//        {
//            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
//            Dispose(true);
//            // TODO: uncomment the following line if the finalizer is overridden above.
//            // GC.SuppressFinalize(this);
//        }

//        #endregion

//        #region IHostedService

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion IHostedService

// */
