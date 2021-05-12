using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Application.Usecases.Files.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Core.Application.Usecases.Templates.Commands
{
    public class FillTemplateCommand: IRequest<FileViewModel>
    {
        [JsonIgnore]
        public string TemplateName { get; set; }

        public object Data { get; set; }

        private class Handler: IRequestHandler<FillTemplateCommand, FileViewModel>
        {
            private readonly IMisDbContext _context;
            private readonly ITemplateService _templateService;

            public Handler(IMisDbContext context, ITemplateService templateService)
            {
                _context = context;
                _templateService = templateService;
            }

            public async Task<FileViewModel> Handle(FillTemplateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var template = await _context.Templates.AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Name == request.TemplateName, cancellationToken)
                        ?? throw new Exception($"Шаблон с именем {request.TemplateName} не найден.");

                    var resultDoc = await _templateService
                        .FillTemplate(template.Content, JObject.FromObject(request.Data));

                    return new FileViewModel
                    {
                        FileName = template.FileName,
                        ContentType = template.ContentType,
                        Content = resultDoc
                    };
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}