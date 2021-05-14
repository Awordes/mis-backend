using System;
using Core.Application.Common.Mapping;
using Core.Domain;

namespace Core.Application.Usecases.Templates.ViewModels
{
    public class TemplateViewModel: IMapFrom<Template>
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }
        
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}