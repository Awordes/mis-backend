using System.Threading.Tasks;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Logging.Queries;
using Core.Application.Usecases.Logging.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    public class VsdProcessTransactionController: BaseController
    {
        /// <summary>
        /// Получить список операций текущего пользователя
        /// </summary>
        [Authorize(Roles = "client")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PagedResult<VsdProcessTransactionViewModel>>> List([FromQuery] GetVsdProcessTransactionsQuery query)
        {         
            return await Mediator.Send(query);
        }
    }
}