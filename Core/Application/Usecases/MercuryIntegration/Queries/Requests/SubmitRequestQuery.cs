﻿using Core.Application.Common;
using Core.Domain.MercuryModels.Bodies;
using Core.Domain.MercuryModels.Data;
using Core.Domain.MercuryModels.Requests;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Application.Usecases.MercuryIntegration.Queries.Requests
{
    public class SubmitRequestQuery: IRequest<object>
    {
        public DataAbstract Data { get; set; }

        public XmlSerializerNamespaces Namespaces { get; set; }

        public class Handler : IRequestHandler<SubmitRequestQuery, object>
        {
            private readonly MercuryConstants _mercuryConstantsOption;

            private readonly IMediator _mediator;

            public Handler(IMediator mediator, IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _mediator = mediator;
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
            }

            public async Task<object> Handle(SubmitRequestQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var submitRequestBody = new SubmitRequestBody
                    {
                        submitApplicationRequest = new SubmitApplicationRequest
                        {
                            apiKey = _mercuryConstantsOption.ApiKey,
                            application = new Domain.MercuryModels.Common.Application
                            {
                                serviceId = _mercuryConstantsOption.ServiceId,
                                issuerId = _mercuryConstantsOption.IssuerId,
                                issueDate = DateTime.Now,
                                data = request.Data
                            }
                        }
                    };


                    request.Namespaces.Add("apldef", "http://api.vetrf.ru/schema/cdm/application/ws-definitions");
                    request.Namespaces.Add("apl", "http://api.vetrf.ru/schema/cdm/application");

                    return await _mediator.Send(new EnvelopeWrappingQuery { 
                        Body = submitRequestBody,
                        Namespaces = request.Namespaces
                    });
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
