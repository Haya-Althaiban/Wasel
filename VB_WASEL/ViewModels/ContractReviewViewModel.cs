
namespace Wasel.ViewModels.SellerVMs.ContractVMs
{
    public class ContractReviewViewModel
    {
        public int ContractId { get; set; }
        public int BidId { get; set; }
        public string TenderTitle { get; set; }
        public string TenderDescription { get; set; }

        // Buyer info
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
        public string BuyerPhone { get; set; }

        // Contract info
        public decimal ContractValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliverySchedule { get; set; }
        public string ContractDocumentUrl { get; set; }

        // Criteria
        public List<CriteriaViewModel> Criteria { get; set; } = new List<CriteriaViewModel>();

        // Payments
        public List<PaymentSummaryViewModel> Payments { get; set; } = new List<PaymentSummaryViewModel>();

        // Statistics
        public decimal TotalPayments { get; set; }
        public decimal RemainingAmount { get; set; }
        public double CompletionPercentage { get; set; }
    }
}
