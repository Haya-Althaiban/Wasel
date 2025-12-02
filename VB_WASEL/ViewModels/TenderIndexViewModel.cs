
namespace Wasel.ViewModels.SellerVMs.TenderVMs
{
    public class TenderIndexViewModel
    {
        public List<TenderListItemViewModel> Tenders { get; set; } = new List<TenderListItemViewModel>();
        public TenderStatsViewModel TenderStats { get; set; } = new TenderStatsViewModel();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
