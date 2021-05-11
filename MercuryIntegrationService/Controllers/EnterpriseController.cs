using System;
using System.Threading.Tasks;
using Core.Application.Usecases.Enterprises.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    [Authorize(Roles = "admin, client")]
    public class EnterpriseController: BaseController
    {
        /// <summary>
        /// Создать предприятие
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateEnterpriseCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Обновить предприятие
        /// </summary>
        [HttpPut("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEnterpriseCommand command)
        {
            command.EnterpriseId = id;
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Удалить предприятие
        /// </summary>
        [HttpDelete("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Delete(Guid id)
        {
            await Mediator.Send(new DeleteEnterpriseCommand { EnterpriseId = id });
            return NoContent();
        }
    }
}