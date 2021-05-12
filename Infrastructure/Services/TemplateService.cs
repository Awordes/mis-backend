using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Services;
using Newtonsoft.Json.Linq;
using OpenXmlPowerTools;

namespace Infrastructure.Services
{
    public class TemplateService: ITemplateService
    {
        public Task<byte[]> FillTemplate(byte[] template, JObject data)
        {
            try
            {
                var doc = new WmlDocument(template.Length.ToString(), template);
                
                foreach (var property in data.Values())
                {
                    var key = $"[[{property.Path}]]";
                    var str = "";
                    switch (property.Type)
                    {
                        case JTokenType.Array:
                            str = property.Values<string>()
                                .Aggregate("", (current, value) => current + value + "; ");
                            
                            
                            doc = doc.SearchAndReplace(key, str, true);
                            break;
                        case JTokenType.None:
                        case JTokenType.Integer:
                        case JTokenType.Float:
                        case JTokenType.String:
                        case JTokenType.Boolean:
                        case JTokenType.Null:
                        case JTokenType.Undefined:
                        case JTokenType.Date:
                        case JTokenType.Raw:
                        case JTokenType.Bytes:
                        case JTokenType.Guid:
                        case JTokenType.Uri:
                        case JTokenType.TimeSpan:
                            str = property.Value<string>();
                            doc = doc.SearchAndReplace(key, string.IsNullOrEmpty(str) ? " " : str, true);
                            break;
                    }
                }

                return Task.FromResult(doc.DocumentByteArray);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}