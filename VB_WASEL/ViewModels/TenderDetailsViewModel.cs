namespace Wasel.ViewModels.SellerVMs.TenderVMs
{
    public class TenderDetailsViewModel
    {
        public int TenderId { get; set; }
        public string TenderTitle { get; set; }
        public string TenderDescription { get; set; }
        public decimal TenderBudget { get; set; }
        public string TenderStatus { get; set; }
        public DateOnly? PublishDate { get; set; }
        public DateOnly? SubmissionDeadline { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Buyer info
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerCity { get; set; }
        public string BuyerAddress { get; set; }

        // Criteria
        public List<CriteriaViewModel> Criteria { get; set; } = new List<CriteriaViewModel>();

        // Existing bid (if any)
        public ExistingBidViewModel ExistingBid { get; set; }

        // Days remaining
        public int DaysRemaining { get; set; }
        public bool IsDeadlineClose { get; set; }
    }
}
