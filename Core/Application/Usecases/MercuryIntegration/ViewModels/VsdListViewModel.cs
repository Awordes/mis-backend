using System.Collections.Generic;

namespace Core.Application.Usecases.MercuryIntegration.ViewModels
{
    public class VsdListViewModel
    {
        public List<VsdViewModel> result { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
        
        public long PageCount { get; set; }

        public long ElementCount { get; set; }
    }
}