using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Core.Application.Common.Services
{
    public interface ITemplateService
    {
        Task<byte[]> FillTemplate(byte[] template, JObject data);
    }
}