using Bb.ReminderStore.Sgbd;

namespace Bb.BusAction.Services
{


    /// <summary>
    /// Manage in/out into storage of the reminder
    /// </summary>
    public class ReminderStore : ReminderStoreSqlServer
    {

        public ReminderStore(ReminderConfiguration configuration)
            : base(configuration.ConnectionString, ActionBusConstants.SqlproviderInvariantName, configuration.WakeUpIntervalSeconds)
        {

        }

    }

}
