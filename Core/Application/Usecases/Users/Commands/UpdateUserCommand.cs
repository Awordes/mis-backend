using System;
using System.Text.Json.Serialization;
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
    public class UpdateUserCommand: IRequest, IMapTo<User>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonIgnore]
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        
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

        private class Handler: IRequestHandler<UpdateUserCommand>
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
            
            public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    _mapper.Map(request, user);

                    (await _userManager.UpdateAsync(user)).CheckResult();
                    
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