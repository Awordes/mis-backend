using System.Threading.Tasks;
using Core.Application.Usecases.Applicants.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    public class ApplicantController: BaseController
    {
        /// <summary>
        /// Создать заявителя
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateApplicantCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}