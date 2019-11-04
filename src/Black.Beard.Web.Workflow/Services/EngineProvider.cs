using Bb.Brokers;
using Bb.Workflows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bb.Workflows.Services
{

    public class EngineProvider : IDisposable
    {

        public EngineProvider(EngineGeneratorConfiguration configuration)
        {
            this._configuration = configuration;
            this._last = new Queue<WorkflowEngine>();
        }


        public void Add(string domain, string path)
        {
            
            var f = new EngineFactory(this._configuration)
                .SetRootPath(path);

            if (_factories.TryGetValue(domain, out EngineFactory factory))
            {
                this._last.Enqueue(factory.Clean());
                _factories[domain] = f;
            }
            else
                _factories.Add(domain, f);
        }


        public void Refresh(string domain)
        {
            if (_factories.TryGetValue(domain, out EngineFactory factory))
                lock (_lock2)
                {
                    var last = factory.Refresh();
                    if (last != null)
                    {
                        this._last.Enqueue(last);
                        Task.Run(() => RemoveLastEngines());
                    }
                }
            else
                throw new NullReferenceException($"no workflow engine configurated for domain '{domain}'");

        }


        public void Stop()
        {

            this._stoping = true;
            var keys = _factories.Keys.ToList();

            lock (_lock2)
            {

                foreach (var key in keys)
                {
                    var f = _factories[key];
                    _factories.Remove(key);
                    if (f.Initialized)
                        this._last.Enqueue(f.Refresh());
                }

                RemoveLastEngines();

            }

        }

        public WorkflowEngine Get(string domain)
        {

            if (this._stoping)
            {
                Trace.WriteLine($"Current application is stoping. Endservice to provide new Workflow engine");
                return null;
            }

            if (_factories.TryGetValue(domain, out EngineFactory factory))
                return factory.Get();

            throw new NullReferenceException($"no workflow engine configurated on domain {domain}");

        }

        private void RemoveLastEngines()
        {
            lock (_lock2)
                while (_last.Count > 0)
                {
                    var item = _last.Dequeue();
                    var toEnd = DateTimeOffset.Now.AddMinutes(WaitMinuteForExit);
                    while (!item.CanBeRemoved && toEnd > DateTimeOffset.Now)
                        Thread.Yield();
                }
        }

        public int WaitMinuteForExit { get; set; } = 2;

        private Dictionary<string, EngineFactory> _factories = new Dictionary<string, EngineFactory>();

        public EngineGeneratorConfiguration _configuration { get; }

        private readonly Queue<WorkflowEngine> _last;
        private volatile object _lock2 = new object();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls
        private bool _stoping;

        protected virtual void Dispose(bool disposing)
        {

            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~EngineProvider()
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

    }


}
