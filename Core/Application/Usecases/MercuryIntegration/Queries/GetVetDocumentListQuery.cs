using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Core.Application.Usecases.MercuryIntegration.ViewModels;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class GetVetDocumentListQuery : IRequest<VsdListViewModel>
    {
        /// <summary>
        /// Номер страницы
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Статус ВСД
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Тип ВСД
        /// </summary>
        public int Type { get; set; }

        private class Handler : IRequestHandler<GetVetDocumentListQuery, VsdListViewModel>
        {
            private readonly IMercuryService _mercuryService;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMercuryService mercuryService,
                IHttpContextAccessor httpContextAccessor,
                UserManager<User> userManager)
            {
                _mercuryService = mercuryService;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
            }

            public async Task<VsdListViewModel> Handle(GetVetDocumentListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

                    var user = await _userManager.FindByNameAsync(userName)
                        ?? throw new Exception($@"Пользователь с именем {userName} не найден.");
                    
                    var count = request.PageSize;

                    var offset = request.PageSize * (request.Page - 1);

                    var vm =  await _mercuryService.GetVetDocumentList(
                        "a10003",
                        user,
                        count,
                        offset,
                        request.Type,
                        request.Status
                    );

                    vm.PageSize = request.PageSize;
                    vm.CurrentPage = request.Page;
                    vm.PageCount = vm.ElementCount / vm.PageSize + (vm.ElementCount % vm.PageSize > 0 ? 1 : 0);
                    
                    return vm;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
