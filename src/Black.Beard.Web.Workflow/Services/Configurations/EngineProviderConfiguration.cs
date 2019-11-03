using System.ComponentModel;

namespace Bb.Workflows.Services
{
    public class EngineProviderConfiguration
    {

        [Description("name of the workflow domain")]
        public string Domain { get; set; }
        
        [Description("path where the workflow configuration can be found")]
        public string Path { get; set; }
    
    }


}
