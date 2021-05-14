using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Application.Usecases.Files.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Users.Queries
{
    public class GetUserVetisStatementQuery: IRequest<FileViewModel>
    {
        public Guid UserId { get; set; }
        
        private class Handler: IRequestHandler<GetUserVetisStatementQuery, FileViewModel>
        {
            private readonly UserManager<User> _userManager;
            private readonly IFileService _fileService;

            public Handler(UserManager<User> userManager,
                IFileService fileService)
            {
                _userManager = userManager;
                _fileService = fileService;
            }

            public async Task<FileViewModel> Handle(GetUserVetisStatementQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.Users.AsNoTracking()
                            .Include(x => x.VetisStatement)
                            .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken)
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    if (user.VetisStatement is null)
                        throw new Exception("Заявление пользователя не найдено");

                    return new FileViewModel
                    {
                        FileName = user.VetisStatement.Name,
                        ContentType = user.VetisStatement.ContentType,
                        Content = await _fileService.Load(user.VetisStatement.Path) 
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