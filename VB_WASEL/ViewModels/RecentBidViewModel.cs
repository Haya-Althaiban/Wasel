namespace Wasel.ViewModels.SellerVMs.DashboardVMs
{
    public class RecentBidViewModel
    {
        public int BidId { get; set; }
        public string TenderTitle { get; set; }
        public string BuyerName { get; set; }
        public decimal ProposedPrice { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public string Status { get; set; }
    }
}
