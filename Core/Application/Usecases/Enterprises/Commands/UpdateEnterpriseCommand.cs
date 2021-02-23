using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Usecases.Enterprises.Commands
{
    public class UpdateEnterpriseCommand: IRequest, IMapTo<Enterprise>
    {
        [JsonIgnore]
        public Guid EnterpriseId { get; set; }
        
        public Guid? UserId { get; set; }

        public string MercuryId { get; set; }

        public string Name { get; set; }
        
        private class Handler: IRequestHandler<UpdateEnterpriseCommand>
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
            
            public async Task<Unit> Handle(UpdateEnterpriseCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var entity = await _context.Enterprises
                            .FirstOrDefaultAsync(x => x.Id == request.EnterpriseId, cancellationToken)
                        ?? throw new Exception($@"Предприятие с идентификатором {request.EnterpriseId} не найдено.");

                    _mapper.Map(request, entity);

                    if (request.UserId.HasValue)
                    {
                        var user = await _userManager.FindByIdAsync(request.UserId.Value.ToString())
                            ?? throw new Exception($@"Пользователь с идентификатором {request.UserId} не найден.");

                        entity.User = user;
                    }
                    
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