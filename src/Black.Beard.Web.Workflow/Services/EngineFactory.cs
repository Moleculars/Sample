using Bb.Brokers;
using Bb.Workflows;
using Bb.Workflows.Models;
using System;

namespace Bb.Workflows.Services
{
    public class EngineFactory
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="path"></param>
        public EngineFactory(EngineGeneratorConfiguration configuration)
        {
            this._engineCreator = new EngineGenerator<RunContext>(configuration);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="path"></param>
        public EngineFactory SetRootPath(string path)
        {
            this._path = path;
            this._engineCreator.SetPath(this._path);
            return this;
        }


        /// <summary>
        /// Return current <see cref="WorkflowEngine"/>
        /// </summary>
        /// <returns></returns>
        public WorkflowEngine Get()
        {

            if (this._engine == null)
                Refresh();

            return this._engine;

        }

        public bool Initialized { get => this._engine != null; }

        /// <summary>
        /// Refresh current <see cref="WorkflowEngine"/>
        /// </summary>
        /// <returns></returns>
        public WorkflowEngine Refresh()
        {

            WorkflowEngine result = null;

            lock (_lock1)
            {
                result = this._engine;
                this._engine = Create();
            }

            return result;

        }

        /// <summary>
        /// Remove current <see cref="WorkflowEngine"/>
        /// </summary>
        /// <returns></returns>
        public WorkflowEngine Clean()
        {

            WorkflowEngine result = null;

            lock (_lock1)
            {
                result = this._engine;
                this._engine = null;
            }

            return result;

        }

        private WorkflowEngine Create()
        {
            return this._engineCreator.CreateEngine();
        }

        private volatile object _lock1 = new object();
        private string _path;
        private readonly EngineGenerator<RunContext> _engineCreator;
        private WorkflowEngine _engine;

    }


}
