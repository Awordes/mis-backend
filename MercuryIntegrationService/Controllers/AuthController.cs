using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Application.Usecases.Auth.Commands;

namespace MercuryIntegrationService.Controllers
{
    public class AuthController: BaseController
    {
        /// <summary>
        /// Авторизация
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Сброс авторизации
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Logout()
        {
            await Mediator.Send(new LogoutCommand());
            return NoContent();
        }

        /// <summary>
        /// Проверка авторизации
        /// </summary>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public IActionResult LoginCheck()
        {
            return Ok();
        }
    }
}
