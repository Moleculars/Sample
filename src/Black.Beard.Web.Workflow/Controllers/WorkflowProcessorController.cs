using System.Threading.Tasks;
using Bb;
using Bb.Workflows.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bb.Workflows.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class WorkflowProcessorController : ControllerBase
    {

        public WorkflowProcessorController(WebEngineProvider provider)
        {
            this._provider = provider;        
        }

        [HttpPost("Push/{domain}")]
        public Task Post(string domain, [FromBody] string payload)
        {

            var wrk = _provider.Get(domain);

            if (wrk == null)
            {
                wrk.EvaluateEvent(payload);
                return Task.CompletedTask;
            }

            throw new TaskCanceledException();

        }

        [HttpGet("Refresh/{domain}")]
        public Task Refresh(string domain)
        {
            _provider.Refresh(domain);
            return Task.CompletedTask;
        }

        private readonly WebEngineProvider _provider;

    }

}