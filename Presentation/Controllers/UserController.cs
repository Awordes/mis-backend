using System;
using Core.Application.Usecases.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Application.Usecases.Users.Commands.ChangePassword;
using Core.Application.Usecases.Users.Commands.UpdateUser;
using Core.Application.Usecases.Users.Queries.GetCurrentUser;
using Core.Application.Usecases.Users.Queries.GetUser;
using Core.Application.Usecases.Users.ViewModels;

namespace Presentation.Controllers
{
    [Authorize]
    public class UserController: BaseController
    {
        /// <summary>
        /// Создать пользователя
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        [HttpPut("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            command.UserId = id;
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Получить пользователя
        /// </summary>
        [HttpGet("/[controller]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> Get([FromQuery] GetUserQuery query)
        {
            return await Mediator.Send(query);
        }
        
        /// <summary>
        /// Сменить пароль пользователя
        /// </summary>
        [HttpPost("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> ChangePassword(Guid id, [FromBody] UserChangePasswordCommand command)
        {
            command.UserId = id;
            
            await Mediator.Send(command);

            return NoContent();
        }
        
        /// <summary>
        /// Получить текущего пользователя
        /// </summary>
        [HttpGet("/[controller]/Current")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> GetCurrent()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }
    }
}
