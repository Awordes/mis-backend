using Core.Application.Usecases.MercuryIntegration.Queries.Methods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class VsdController: BaseController
    {
        [Authorize]
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetVsdList(GetVetDocumentListQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}
