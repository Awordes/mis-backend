using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Usecases.Files.ViewModels;
using Core.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Templates.Queries
{
    public class GetTemplateFileQuery: IRequest<FileViewModel>
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
        
        private class Handler: IRequestHandler<GetTemplateFileQuery, FileViewModel>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IMisDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<FileViewModel> Handle(GetTemplateFileQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var template = new Template();
                    
                    if (request.Id.HasValue)
                        template = await _context.Templates.AsNoTracking()
                                       .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                                   ?? throw new Exception($@"Шаблон с идентификатором {request.Id} не найден.");
                    else if (request.Name is not null)
                        template = await _context.Templates.AsNoTracking()
                                       .FirstOrDefaultAsync(x => x.Name.Equals(request.Name), cancellationToken)
                                   ?? throw new Exception($@"Шаблон с системным кодом {request.Name} не найден.");
                    else if (request.Title is not null)
                        template = await _context.Templates.AsNoTracking()
                                       .FirstOrDefaultAsync(x => x.Title.Equals(request.Title), cancellationToken)
                                   ?? throw new Exception($@"Шаблон с наименованием {request.Title} не найден.");

                    return _mapper.Map<FileViewModel>(template);
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