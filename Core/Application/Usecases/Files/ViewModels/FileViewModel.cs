using Core.Application.Common.Mapping;
using Core.Domain;

namespace Core.Application.Usecases.Files.ViewModels
{
    public class FileViewModel: IMapFrom<FileBase>
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}