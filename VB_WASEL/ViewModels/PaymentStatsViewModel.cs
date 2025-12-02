namespace Wasel.ViewModels.SellerVMs.PaymentVMs
{
    public class PaymentStatsViewModel
    {
        public decimal TotalPayments { get; set; }
        public int PendingPayments { get; set; }
        public int CompletedPayments { get; set; }
        public decimal TotalCommission { get; set; }
        public int PaymentCount { get; set; }
    }
}
