using Wasel.ViewModels.BuyerVMs;

namespace Wasel.ViewModels.SellerVMs.DashboardVMs
{
    public class DashboardViewModel
    {
        public DashboardStatsViewModel Stats { get; set; } = new DashboardStatsViewModel();
        public List<RecentBidViewModel> RecentBids { get; set; } = new List<RecentBidViewModel>();
        public int OpenTendersCount { get; set; }
        public ChartDataViewModel ChartData { get; set; } = new ChartDataViewModel();
        public string SellerName { get; set; }
    }
}
