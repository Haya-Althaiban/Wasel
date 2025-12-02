namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class RecentBidActivityViewModel
    {
        public int BidId { get; set; }
        public string TenderTitle { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public decimal ProposedPrice { get; set; }
        public string Status { get; set; }
    }
}
