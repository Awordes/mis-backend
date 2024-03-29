﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Users.Commands;
using Core.Application.Usecases.Users.Queries;
using Core.Application.Usecases.Users.ViewModels;

namespace MercuryIntegrationService.Controllers
{
    [Authorize]
    public class UserController: BaseController
    {
        /// <summary>
        /// Создать пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateUserCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Получить пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("/[controller]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> Get([FromQuery] GetUserQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Обновить пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
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
        /// Удалить пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpDelete("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteUserCommand { UserId = id });
            return NoContent();
        }
        
        /// <summary>
        /// Получить текущего пользователя
        /// </summary>
        [HttpGet("/[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> Current()
        {
            return await Mediator.Send(new GetCurrentUserQuery());
        }
        
        /// <summary>
        /// Получить постраничный список пользователей
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PagedResult<UserViewModel>>> List([FromQuery] GetUserListQuery query)
        {         
            return await Mediator.Send(query);
        }
        
        /// <summary>
        /// Получить список ролей пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserRolesViewModel>> Roles(Guid id)
        {         
            return await Mediator.Send(new GetUserRolesQuery {UserId = id});
        }
        
        /// <summary>
        /// Получить список предприятий пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserEnterprisesViewModel>> Enterprises(Guid id)
        {         
            return await Mediator.Send(new GetUserEnterprisesQuery {UserId = id});
        }
        
        /// <summary>
        /// Сменить пароль пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
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
        /// Редактировать роли пользователя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<UserViewModel>> EditRoles(Guid id, [FromBody] EditUserRolesCommand command)
        {
            command.UserId = id.ToString();            
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Прикрепить заявление Ветис.API
        /// </summary>
        [Authorize]
        [HttpPost("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UploadVetisStatement(Guid id, IFormFile file)
        {
            await Mediator.Send(new UploadVetisStatementCommand
            {
                UserId = id,
                FormFile = file
            });
            return NoContent();
        }
        
        /// <summary>
        /// Создать пользователя из заявителя
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost("/[controller]/[action]/{applicantId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> CreateFromApplicant(Guid applicantId)
        {
            await Mediator.Send(new CreateUserFromApplicantCommand { ApplicantId = applicantId });
            return NoContent();
        }
        
        /// <summary>
        /// Получить заявление Ветис.API пользователя
        /// </summary>
        [Authorize(Roles = "admin,client")]
        [HttpPost("/[controller]/{id:guid}/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> VetisStatement(Guid id)
        {
            var result = await Mediator.Send(new GetUserVetisStatementQuery { UserId = id });

            return File(result.Content, result.ContentType, result.FileName);
        }
    }
}
