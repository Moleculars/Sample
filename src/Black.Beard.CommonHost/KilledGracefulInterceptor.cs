using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace Bb
{

    /// <summary>
    /// Wrap process stop application
    /// </summary>
    public class KilledGracefulInterceptor : IDisposable
    {

        #region Ctor

        static KilledGracefulInterceptor()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit; ;
            AssemblyLoadContext.Default.Unloading += Default_Unloading;
        }

        public KilledGracefulInterceptor()
        {
            KilledGracefulInterceptor._eventHandler += Stop_Impl;
        }

        #endregion Ctor

        #region Stop

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Stop();
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            Stop();
        }

        private void Stop_Impl(object sender, EventArgs e)
        {
            StopAction?.Invoke();
        }

        public static void Stop()
        {

            if (!_stoped)
                lock (_lock)
                    if (!_stoped)
                    {
                        _stoped = true;
                        _eventHandler?.Invoke(null, new EventArgs());
                    }
        
        }

        
        public Action StopAction { get; set; }

        #endregion Stop

        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {

                if (disposing)
                    KilledGracefulInterceptor._eventHandler -= Stop_Impl;

                disposedValue = true;

            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KillInterceptor()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion

        private static EventHandler<EventArgs> _eventHandler;
        private static object _lock = new object();
        private bool disposedValue = false; // To detect redundant calls
        private static bool _stoped;

    }


}
