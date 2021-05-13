using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common;
using Core.Application.Common.Services;
using Core.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Commands
{
    public class UploadVetisStatementCommand: IRequest
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Файл заявления
        /// </summary>
        public IFormFile FormFile { get; set; }

        private class Handler: IRequestHandler<UploadVetisStatementCommand>
        {
            private readonly IMisDbContext _context;
            private readonly IFileService _fileService;

            public Handler(IMisDbContext context, IFileService fileService)
            {
                _context = context;
                _fileService = fileService;
            }

            public async Task<Unit> Handle(UploadVetisStatementCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _context.Users
                               .Include(x => x.Enterprises)
                               .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                           ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");
                    
                    await using var memoryStream = new MemoryStream();
                    await request.FormFile.CopyToAsync(memoryStream, cancellationToken);
                    
                    var path = await _fileService.Save(request.FormFile.FileName, memoryStream.ToArray());

                    user.VetisStatement = new MercuryFileInfo
                    {
                        Name = request.FormFile.FileName,
                        Path = path,
                        ContentType = request.FormFile.ContentType
                    };

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