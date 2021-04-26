using System.Threading.Tasks;
using Core.Application.Usecases.Templates.Commands;
using Core.Application.Usecases.Templates.Queries;
using Core.Application.Usecases.Templates.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MercuryIntegrationService.Controllers
{
    public class TemplateController: BaseController
    {
        /// <summary>
        /// Создать шаблон
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPost("/[controller]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Create([FromForm] CreateTemplateCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Получить шаблон
        /// </summary>
        [HttpGet("/[controller]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<TemplateViewModel>> Get([FromQuery] GetTemplateQuery query)
        {
            return await Mediator.Send(query);
        }
        
        /// <summary>
        /// Получить файл шаблона
        /// </summary>
        [HttpGet("/[controller]/Download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Download([FromQuery] GetTemplateFileQuery query)
        {
            var fileViewModel = await Mediator.Send(query);
            return File(fileViewModel.Content, fileViewModel.ContentType, fileViewModel.FileName);
        }
    }
}