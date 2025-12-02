namespace Wasel.ViewModels.SellerVMs.TenderVMs
{
    public class ExistingBidViewModel
    {
        public int BidId { get; set; }
        public decimal ProposedPrice { get; set; }
        public string ProposedTimeline { get; set; }
        public string BidDescription { get; set; }
        public DateOnly? SubmissionDate { get; set; }
    }
}
