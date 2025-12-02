namespace Wasel.ViewModels.SellerVMs.PaymentVMs
{
    public class PaymentListItemViewModel
    {
        public int PaymentNum { get; set; }
        public int ContractId { get; set; }
        public string TenderTitle { get; set; }
        public string BuyerName { get; set; }
        public DateOnly? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal BuyerCommission { get; set; }
        public decimal SellerCommission { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentStatus { get; set; }
        public bool HasInvoice { get; set; }
        public int? InvoiceNum { get; set; }
    }
}
