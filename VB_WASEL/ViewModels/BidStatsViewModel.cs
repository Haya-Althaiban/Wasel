namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class BidStatsViewModel
    {
        public int TotalBids { get; set; }
        public int ActiveBids { get; set; }
        public int AwardedBids { get; set; }
        public double SuccessRate { get; set; }
        public List<RecentBidActivityViewModel> RecentActivity { get; set; } = new List<RecentBidActivityViewModel>();
    }
}
