using System.Threading.Tasks;
using Core.Application.Usecases.Admin.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    [Authorize]
    public class AdminController: BaseController
    {
        /// <summary>
        /// Создать пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> StartAutoVsdProcessing()
        {
            await Mediator.Send(new StartAutoVsdProcessingCommand());
            return NoContent();
        }
    }
}