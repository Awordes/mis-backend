﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Extensions;
using Core.Application.Common.Pagination;
using Core.Application.Usecases.Applicants.ViewModels;
using Core.Domain.Applicants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Application.Usecases.Applicants.Queries
{
    public class GetApplicantListQuery: IRequest<PagedResult<ApplicantViewModel>>
    {
        /// <summary>
        /// Количество страниц
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество элементов на странице
        /// </summary>
        public int PageSize { get; set; }
        
        private class Handler: IRequestHandler<GetApplicantListQuery, PagedResult<ApplicantViewModel>>
        {
            private readonly IMisDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

            public Handler(IMisDbContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<PagedResult<ApplicantViewModel>> Handle
                (GetApplicantListQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var query = _context.Applicants.AsNoTracking().OrderBy(x => x.CreationDate);

                    return await query.GetPagedAsync<Applicant, ApplicantViewModel>
                        (request.Page, request.PageSize, _mapper, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                    throw;
                }
            }
        }
    }
}