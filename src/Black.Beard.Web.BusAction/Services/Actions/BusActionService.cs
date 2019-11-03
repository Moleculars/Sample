using Bb.ActionBus;
using Bb.ComponentModel.Attributes;
using Bb.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bb.BusAction.Services
{

    [ExposeClass(Context = ActionBusContants.BusinessActionBus, Name = "Reminder")]
    public class BusActionService
    {

        public BusActionService(ReminderService reminder)
        {
            this._reminder = reminder;
        }


        [ExposeMethod(Context = ActionBusContants.BusinessActionBus, DisplayName = "Watch")]
        public void WatchToReminder(ActionBusContext ctx, int delayInMinute, string message)
        {

            var a = new WakeUpRequestModel()
            {
                Uuid = Guid.NewGuid(),
                Binding = string.Empty,
                DelayInMinute = delayInMinute,
                Message = message,
            };


            _reminder.Watch(a) ;

        }

        [ExposeMethod(Context = ActionBusContants.BusinessActionBus, DisplayName = "Cancel")]
        public void CancelReminder(ActionBusContext ctx, string key)
        {

            _reminder.Cancel(Guid.Parse(key));

        }

        private readonly ReminderService _reminder;

    }

}
