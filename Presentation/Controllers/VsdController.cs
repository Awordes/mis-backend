using Core.Application.Usecases.MercuryIntegration;
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
        public async Task<IActionResult> GetVsdList(SendRequestCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
