using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Core.Application.Usecases.Templates.Commands
{
    public class UpdateTemplateCommand: IRequest, IMapTo<Template>
    {
        [JsonIgnore]
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public IFormFile FormFile { get; set; }
        
        private class Handler: IRequestHandler<UpdateTemplateCommand>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IMisDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var template =
                        await _context.Templates.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                        ?? throw new Exception($"Шаблон с идентификатором {request.Id} не найден.");

                    _mapper.Map(request, template);
                    
                    await using var memoryStream = new MemoryStream();
                    await request.FormFile.CopyToAsync(memoryStream, cancellationToken);
                    template.Content = memoryStream.ToArray();

                    template.FileName = request.FormFile.FileName;
                    template.ContentType = request.FormFile.ContentType;

                    await _context.SaveChangesAsync(cancellationToken);
                    
                    return Unit.Value;
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