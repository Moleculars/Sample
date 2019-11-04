using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.Workflows.Services
{

    [ExposeClass(Context = ConstantsCore.Configuration, ExposedType = typeof(EngineGeneratorModel), Name = "EngineGenerator", LifeCycle = IocScopeEnum.Singleton)]
    public class EngineGeneratorModel
    {

        [Description("Name of the publisher for push action")]
        public string PublishToAction { get; set; }

        [Description("Name of the queue of incoming event of workflow")]
        public string WorkflowQueue { get; set; }

        [Description("Name of the dead queue when incoming event integration process fail more of maximum retry")]
        public string PublishDeadQueueWorkflow { get; set; }

        [Description("header name for store domain workflow")]
        public string DomainHeader { get; set; } = "domain";

    }


}
