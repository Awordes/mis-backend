using Core.Application.Usecases.Auth.Queries.Authenticate;
using Core.Application.Usecases.Auth.ViewModels;
using Core.Application.Usecases.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    public class AuthController: BaseController
    {
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<AuthenticationViewModel>> Login([FromQuery] AuthenticateQuery query)
        {            
            return await Mediator.Send(query);
        }
    }
}
