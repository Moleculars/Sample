﻿
using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.Workflows.Services
{

    /// <summary>
    /// this configuration will be appended in IOC of the application
    /// the document that contains the configuration must be called "Workflows"
    /// </summary>
    [ExposeClass(Context = ConstantsCore.Configuration, ExposedType = typeof(EngineConfigurationModel), Name = "Workflows", LifeCycle = IocScopeEnum.Singleton)]
    public class EngineConfigurationModel
    {

        public EngineConfigurationModel()
        {

        }

        [Description("List of workflow configurations by domain")]
        public EngineProviderConfigurations Domains { get; set; }

    }


}
