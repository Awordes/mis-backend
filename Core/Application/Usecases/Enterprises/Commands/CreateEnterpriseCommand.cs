using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Core.Application.Usecases.Enterprises.Commands
{
    public class CreateEnterpriseCommand: IRequest, IMapTo<Enterprise>
    {
        public Guid UserId { get; set; }

        public string MercuryId { get; set; }

        public string Name { get; set; }
        
        private class Handler: IRequestHandler<CreateEnterpriseCommand>
        {
            private readonly UserManager<User> _userManager;
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager,
                IMisDbContext context,
                IMapper mapper)
            {
                _userManager = userManager;
                _context = context;
                _mapper = mapper;
            }
            
            public async Task<Unit> Handle(CreateEnterpriseCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(request.UserId.ToString())
                        ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                    var entity = _mapper.Map<Enterprise>(request);

                    entity.User = user;

                    _context.Enterprises.Add(entity);

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