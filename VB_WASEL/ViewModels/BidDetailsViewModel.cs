using Wasel.ViewModels.BuyerVMs;

namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class BidDetailsViewModel
    {
        public int BidId { get; set; }
        public int TenderId { get; set; }
        public string TenderTitle { get; set; }
        public string TenderDescription { get; set; }
        public decimal TenderBudget { get; set; }
        public string TenderStatus { get; set; }
        public DateOnly? SubmissionDeadline { get; set; }

        // Buyer info
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }

        // Bid info
        public decimal ProposedPrice { get; set; }
        public string ProposedTimeline { get; set; }
        public string BidDescription { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsRejected { get; set; }
        public DateTime? RejectedAt { get; set; }
        public bool HasContract { get; set; }
        public int? ContractId { get; set; }

        // Criteria
        public List<CriteriaViewModel> Criteria { get; set; } = new List<CriteriaViewModel>();

        // Comparison
        public BidComparisonViewModel BidComparison { get; set; }
        public int DaysSinceSubmission { get; set; }
    }
}
