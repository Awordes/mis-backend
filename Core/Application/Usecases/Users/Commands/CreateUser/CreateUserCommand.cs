﻿using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain.Users;
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
        public string Login { get; set; }

        public string HashedPassword { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateUserCommand, User>()
                .ForMember(x => x.UserName, opt => opt.MapFrom(e => e.Login))
                ;
        }

        private class Handler : IRequestHandler<CreateUserCommand>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;
            private readonly UserManager<User> _userManager;

            public Handler(
                IMisDbContext context,
                IMapper mapper,
                UserManager<User> userManager)
            {
                _context = context;
                _mapper = mapper;
                _userManager = userManager;
            }

            public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var user = _mapper.Map<User>(request);

                    var result = await _userManager.CreateAsync(user,
                        Encoding.UTF8.GetString(Convert.FromBase64String(request.HashedPassword)));

                    if (!result.Succeeded)
                    {
                        var errors = result.Errors.Select(err => $"{err.Code}: {err.Description}").ToArray();
                        throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
                    }

                    return Unit.Value;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
