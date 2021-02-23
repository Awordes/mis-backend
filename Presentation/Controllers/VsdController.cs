using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Core.Application.Usecases.MercuryIntegration.Commands;
using Core.Application.Usecases.MercuryIntegration.Queries;
using Core.Application.Usecases.MercuryIntegration.ViewModels;

namespace Presentation.Controllers
{
    [Authorize]
    public class VsdController: BaseController
    {
        /// <summary>
        /// Получение списка ВСД
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<VsdListViewModel>> GetVsdList([FromQuery] GetVetDocumentListQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Получение статусов ВСД
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetVsdStatuses()
        {
            return await Mediator.Send(new GetVsdStatusesQuery());
        }

        /// <summary>
        /// Получение типов ВСД
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetVsdTypes()
        {
            return await Mediator.Send(new GetVsdTypesQuery());
        }
        
        /// <summary>
        /// Получение ВСД по идентификатору
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<object>> GetVsdByUuid([FromQuery] GetVetDocumentByUuidQuery query)
        {
            return await Mediator.Send(query);
        }
        
        /// <summary>
        /// Погасить ВСД по идентификатору
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FinishVsd([FromBody] FinishVetDocumentCommand command)
        {
            await Mediator.Send(command);

            return NoContent();
        }
    }
}
