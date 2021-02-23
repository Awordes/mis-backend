using System.Threading.Tasks;
using Core.Application.Usecases.Enterprises.Commands.CreateEnterprise;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class EnterpriseController: BaseController
    {
        /// <summary>
        /// Создать пользователя
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateEnterpriseCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }        
    }
}