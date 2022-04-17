using System.Threading.Tasks;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Applicants.Commands;
using Core.Application.Usecases.Applicants.Queries;
using Core.Application.Usecases.Applicants.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    public class ApplicantController: BaseController
    {
        /// <summary>
        /// Создать заявителя
        /// </summary>
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create(CreateApplicantCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Получить список заявителей
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpGet("/[controller]/List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PagedResult<ApplicantViewModel>>> GetList([FromQuery] GetApplicantListQuery query)
        {
            return await Mediator.Send(query);;
        }
    }
}