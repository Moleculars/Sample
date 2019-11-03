using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.BusAction.Services
{


    [ExposeClass(Context = ConstantsCore.Configuration, ExposedType = typeof(ActionBusBrokerConfiguration), Name = "ActionBusBrokerConfiguration", LifeCycle = IocScopeEnum.Singleton)]
    public class ActionBusBrokerConfiguration
    {

        [Description("Queue of action action to execute")]
        public string ActionBusQueue { get; set; }

        [Description("Queue to push action failed to execute")]
        public string DeadQueueAction { get; set; }

        [Description("Queue to push result action")]
        public string AcknowledgeExchange { get; set; }

    }

}
