using System;
using System.Threading.Tasks;
using Core.Application.Usecases.Roles.Commands;
using Core.Application.Usecases.Roles.Queries;
using Core.Application.Usecases.Roles.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize(Roles = "admin")]
    public class RoleController: BaseController
    {
        /// <summary>
        /// Создать роль
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateRoleCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Обновить роль
        /// </summary>
        [HttpPut("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
        {
            command.RoleId = id;
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Получить список ролей
        /// </summary>
        [HttpGet("/[controller]/List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<RoleListViewModel>> GetList()
        {
            return await Mediator.Send(new GetRolesQuery());
        }
    }
}