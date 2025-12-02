namespace Wasel.ViewModels.SellerVMs.PaymentVMs
{
    public class PaymentInvoiceViewModel
    {
        public int PaymentNum { get; set; }
        public int InvoiceNum { get; set; }
        public DateOnly? InvoiceDate { get; set; }
        public TimeOnly? InvoiceTime { get; set; }

        // Contract details
        public int ContractId { get; set; }
        public decimal ContractValue { get; set; }

        // Tender details
        public string TenderTitle { get; set; }
        public string TenderDescription { get; set; }

        // Buyer details
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public string BuyerCity { get; set; }
        public string BuyerPhone { get; set; }
        public string BuyerEmail { get; set; }

        // Seller details
        public string SellerName { get; set; }
        public string SellerAddress { get; set; }
        public string SellerCity { get; set; }
        public string SellerPhone { get; set; }

        // Payment details
        public DateOnly? PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public decimal BuyerCommission { get; set; }
        public decimal SellerCommission { get; set; }
        public decimal NetAmount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
