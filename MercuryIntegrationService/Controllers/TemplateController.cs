﻿using System;
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
        [HttpGet("/[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Download([FromQuery] GetTemplateFileQuery query)
        {
            var fileViewModel = await Mediator.Send(query);
            return File(fileViewModel.Content, fileViewModel.ContentType, fileViewModel.FileName);
        }
        
        /// <summary>
        /// Обновить шаблон
        /// </summary>
        [Authorize(Roles = "admin")]
        [HttpPut("/[controller]/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateTemplateCommand command)
        {
            command.Id = id;
            await Mediator.Send(command);
            return NoContent();
        }
        
        /// <summary>
        /// Заполнить шаблон данными
        /// </summary>
        [HttpPost("/[controller]/[action]/{templateName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> FillTemplate(string templateName, [FromBody] FillTemplateCommand query)
        {
            query.TemplateName = templateName;
            var fileViewModel = await Mediator.Send(query);
            return File(fileViewModel.Content, fileViewModel.ContentType, fileViewModel.FileName);
        }
    }
}