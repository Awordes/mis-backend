using Core.Application.Usecases.MercuryIntegration.Queries.Methods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    public class VsdController: BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetVsdList([FromQuery] GetVetDocumentListQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
