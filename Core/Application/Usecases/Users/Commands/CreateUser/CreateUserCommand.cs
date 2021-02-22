using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Usecases.Users.Commands.CreateUser
{
    public class CreateUserCommand : IRequest, IMapTo<User>
    {
        public string Login { get; }

        public string Password { get; }
        
        public string Inn { get; set; }

        public string Title { get; set; }

        public string Contact { get; set; }

        public string MercuryLogin { get; set; }

        public string MercuryPassword { get; set; }

        public string ApiLogin { get; set; }

        public string ApiPassword { get; set; }

        public string ApiKey { get; set; }

        public string IssuerId { get; set; }

        public bool EditAllow { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateUserCommand, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(e => e.Login))
                ;
        }

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

                    var result = await _userManager.CreateAsync(user,
                        request.Password);

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(err => $"{err.Code}: {err.Description}").ToArray();
                        throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
                    }

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
