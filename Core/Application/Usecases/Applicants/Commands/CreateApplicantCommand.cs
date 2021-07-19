using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Mapping;
using Core.Domain;
using Core.Domain.Applicants;
using MediatR;

namespace Core.Application.Usecases.Applicants.Commands
{
    public class CreateApplicantCommand: IRequest, IMapTo<Applicant>
    {
        /// <summary>
        /// Имя
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Электронный адрес
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public string Inn { get; set; }

        /// <summary>
        /// Логин Меркурия
        /// </summary>
        public string MercuryLogin { get; set; }

        /// <summary>
        /// Пароль Меркурия
        /// </summary>
        public string MercuryPassword { get; set; }
        
        private class Handler: IRequestHandler<CreateApplicantCommand>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;

            public Handler(IMisDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Unit> Handle(CreateApplicantCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    var applicant = _mapper.Map<Applicant>(request);

                    applicant.Status = ApplicantStatus.New;

                    await _context.Applicants.AddAsync(applicant, cancellationToken);

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