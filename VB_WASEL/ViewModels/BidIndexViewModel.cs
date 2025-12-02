using Wasel.ViewModels.BuyerVMs;

namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class BidIndexViewModel
    {
        public List<BidListItemViewModel> Bids { get; set; } = new List<BidListItemViewModel>();
        public BidStatsViewModel BidStats { get; set; } = new BidStatsViewModel();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }
}
