﻿
using Bb.Brokers;
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Configurations;

namespace Bb.Workflows.Services
{


    [ExposeClass(Context = ConstantsCore.Configuration, ExposedType = typeof(BrokerConfiguration), Filename = "Brokers", LifeCycle = IocScopeEnum.Singleton)]
    public class BrokerConfiguration
    {

        public ServerBrokerConfiguration[] Servers { get; set; }

        public BrokerPublishParameter[] Publishers { get; set; }

        public BrokerSubscriptionParameter[] Subscribers { get; set; }

        public IFactoryBroker GetFactory()
        {

            var factoryBroker = new RabbitFactoryBrokers();

            foreach (var server in Servers)
                factoryBroker.Add(server);

            foreach (var publisher in Publishers)
                factoryBroker.Add(publisher);

            foreach (var subscriber in Subscribers)
                factoryBroker.Add(subscriber);

            return factoryBroker;

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