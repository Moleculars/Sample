using System.Threading.Tasks;
using Bb;
using Bb.ActionBus;
using Microsoft.AspNetCore.Mvc;

namespace Bb.BusAction.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BusActionController : ControllerBase
    {

        public BusActionController(ActionRunner<ActionBusContext> repositories)
        {
            this._repositories = repositories;
        }

        [HttpPost("Push")]
        public Task Post([FromBody] string payload)
        {

            // Push in bus action
            var context = this._repositories.Evaluate(payload);
            
            if (context.Exception == null)
            {

                if (context.Result == null)
                    return Task.CompletedTask;

                return Task.FromResult(context.Result);
            
            }

            throw new TaskCanceledException(context.Exception.Message);

        }


        private readonly ActionRunner<ActionBusContext> _repositories;

    }

}