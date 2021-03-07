using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Extensions;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Users.Commands
{
    public class CreateUserCommand : IRequest, IMapTo<User>
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Название организации
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Контакты пользователя
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// Логин от системы Меркурий
        /// </summary>
        public string MercuryLogin { get; set; }

        /// <summary>
        /// пароль от системы Меркурий
        /// </summary>
        public string MercuryPassword { get; set; }

        /// <summary>
        /// Логин Ветис.API
        /// </summary>
        public string ApiLogin { get; set; }

        /// <summary>
        /// Пароль Ветис.API
        /// </summary>
        public string ApiPassword { get; set; }

        /// <summary>
        /// Ключ Ветис.API
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Идентификатор заявителя
        /// </summary>
        public string IssuerId { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string EnterpriseId { get; set; }

        /// <summary>
        /// Разрешение редактирования
        /// </summary>
        public bool EditAllow { get; set; }

        /// <summary>
        /// Дата, до которой активен пользователь
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        private class Handler : IRequestHandler<CreateUserCommand>
        {
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMapper mapper,
                UserManager<User> userManager)
            {
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _mapper.Map<User>(request);

                    (await _userManager.CreateAsync(user, request.Password)).CheckResult();

                    return Unit.Value;
                }
                catch(Exception e)
                {
                    Console.Write(e);
                    throw;
                }
            }
        }
    }
}
