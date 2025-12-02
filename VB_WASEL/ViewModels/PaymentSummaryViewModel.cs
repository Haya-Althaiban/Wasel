namespace Wasel.ViewModels.SellerVMs.ContractVMs
{
    public class PaymentSummaryViewModel
    {
        public int PaymentNum { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
