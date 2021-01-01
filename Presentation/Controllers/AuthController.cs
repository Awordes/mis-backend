using Core.Application.Usecases.Users.Commands.CreateUser;
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
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
    }
}
