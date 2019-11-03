using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using System.ComponentModel;

namespace Bb.BusAction.Services
{

    [ExposeClass(Context = ConstantsCore.Configuration, Name = "ReminderConfiguration", LifeCycle = IocScopeEnum.Singleton)]
    public class ReminderConfiguration
    {

        /// <summary>
        /// Gets or sets the connection string for connect reminder index.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        [Description("Connection string for connect to reminder storage")]
        public string ConnectionString { get; set; }

        [Description("Wake up interval in seconds for check expirated reminders")]
        public int WakeUpIntervalSeconds { get; set; } = 20;

    }

}
