namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class BidListItemViewModel
    {
        public int BidId { get; set; }
        public int TenderId { get; set; }
        public string TenderTitle { get; set; }
        public string BuyerName { get; set; }
        public decimal ProposedPrice { get; set; }
        public string ProposedTimeline { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool HasContract { get; set; }
        public string TenderStatus { get; set; }
        public DateOnly? TenderDeadline { get; set; }
    }
}
