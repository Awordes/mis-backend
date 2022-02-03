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

namespace Core.Application.Usecases.Templates.Commands
{
    public class CreateTemplateCommand: IRequest, IMapTo<Template>
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public IFormFile FormFile { get; set; }
        
        private class Handler: IRequestHandler<CreateTemplateCommand>
        {
            private readonly IMapper _mapper;
            private readonly IMisDbContext _context;

            public Handler(IMapper mapper, IMisDbContext context)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Unit> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = _mapper.Map<Template>(request);

                    await using var memoryStream = new MemoryStream();
                    await request.FormFile.CopyToAsync(memoryStream, cancellationToken);
                    entity.Content = memoryStream.ToArray();

                    entity.FileName = request.FormFile.FileName;
                    entity.ContentType = request.FormFile.ContentType;

                    _context.Templates.Add(entity);

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