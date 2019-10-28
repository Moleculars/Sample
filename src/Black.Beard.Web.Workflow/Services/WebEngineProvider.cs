using System;

namespace Bb.Workflows.Services
{


    public class WebEngineProvider : EngineProvider
    {

        /// <summary>
        /// IOC inject configuration because EngineConfigurationModel is registered in ioc by ExposeClassAttribute
        /// </summary>
        /// <param name="configuration"></param>
        public WebEngineProvider(EngineGeneratorConfiguration configuration, KilledGracefulInterceptor killer)
            : base(configuration)
        {
            
            this._killer = killer;
            this._killer.StopAction = this.Stop;
         
            var domains = configuration.EngineConfiguration.Domains;
            if (domains != null)
                foreach (EngineProviderConfiguration config in domains)
                    this.Add(config.Domain, config.Path);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                this._killer.Dispose();
        }

        private readonly KilledGracefulInterceptor _killer;

    }


}
