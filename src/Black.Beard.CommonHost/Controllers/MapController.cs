using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Linq;

namespace Bb.CommonHost.Controllers
{

    /// <summary>
    /// Provide the of all routes of the site
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="actionDescriptorCollectionProvider"></param>
        public MapController(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
        }

        /// <summary>
        /// return the lst of routes
        /// </summary>
        /// <returns></returns>
        [HttpGet("routes", Name = "ApiEnvironmentGetAllRoutes")]
        [Produces("application/json")]
        public IActionResult GetAllRoutes()
        {

            var routes = _actionDescriptorCollectionProvider.ActionDescriptors.Items.Select(x => new
            {
                Action = x.RouteValues["action"],
                Controller = x.RouteValues["controller"],
                Name = x.AttributeRouteInfo?.Name ?? string.Empty,
                Template = x.AttributeRouteInfo?.Template ?? string.Empty
            })
            .ToList();

            return Ok(routes);

        }

        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

    }

}





    /*
     * 
     * https://github.com/swagger-api/swagger-codegen
     * 
     * 
        https://bramp.github.io/js-sequence-diagrams/
        https://github.com/bramp/js-sequence-diagrams

        http://flowchart.js.org/
        https://github.com/adrai/flowchart.js
    */
