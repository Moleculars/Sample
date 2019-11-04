using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Dao;
using System.ComponentModel;

namespace Bb.Workflows.Services
{
    [ExposeClass(Context = ConstantsCore.Configuration, ExposedType = typeof(SqlServerManagerConfiguration), Name = "SqlServerManagerConfiguration", LifeCycle = IocScopeEnum.Singleton)]
    public class SqlServerManagerConfiguration
    {

        [Description("Provider invariant name. By default the value is 'sqlClient'")]
        public string ProviderInvariantName { get; set; } = "sqlClient";

        [Description("Connection String")]
        public string ConnectionString { get; set; }

    }

}
