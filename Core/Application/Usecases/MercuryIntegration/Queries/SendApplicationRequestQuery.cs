using Core.Application.Common;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Core.Application.Usecases.MercuryIntegration.Queries
{
    public class SendApplicationRequestQuery: IRequest<XmlDocument>
    {
        public XmlDocument RequestBody { get; set; }

        private class Handler : IRequestHandler<SendApplicationRequestQuery, XmlDocument>
        {
            private readonly MercuryConstants _mercuryConstantsOption;

            public Handler(IOptionsMonitor<MercuryConstants> mercuryConstantOption)
            {
                _mercuryConstantsOption = mercuryConstantOption.CurrentValue;
            }

            public async Task<XmlDocument> Handle(SendApplicationRequestQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_mercuryConstantsOption.RequestUrl);

                    webRequest.Credentials = CredentialCache.DefaultCredentials;

                    var encoded = Convert.ToBase64String(
                        Encoding.GetEncoding("ISO-8859-1")
                            .GetBytes(_mercuryConstantsOption.ApiLogin + ":" + _mercuryConstantsOption.ApiPassword));

                    webRequest.Headers.Add("Authorization", "Basic " + encoded);
                    webRequest.ContentType = "application/xml";

                    webRequest.Method = "POST";

                    using (var stream = new StreamWriter(webRequest.GetRequestStream(), Encoding.UTF8))
                        stream.Write(request.RequestBody.OuterXml);

                    var response = (HttpWebResponse)await webRequest.GetResponseAsync();

                    var xmlResponse = new XmlDocument();
                    xmlResponse.LoadXml(new StreamReader(response.GetResponseStream()).ReadToEnd());

                    return xmlResponse;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}
