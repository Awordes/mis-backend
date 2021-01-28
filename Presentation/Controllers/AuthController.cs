using Core.Application.Usecases.Auth.Commands.Login;
using Core.Application.Usecases.Auth.Commands.Logout;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class AuthController: BaseController
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Logout()
        {
            await Mediator.Send(new LogoutCommand());
            return NoContent();
        }
    }
}
