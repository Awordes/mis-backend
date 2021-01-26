using Core.Application.Usecases.MercuryIntegration.Queries.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class ApplicationController: BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetRequestResponse([FromQuery] ReceiveApplicationResultRequestQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
