namespace Wasel.ViewModels.SellerVMs.ContractVMs
{
    public class ContractListItemViewModel
    {
        public int ContractId { get; set; }
        public int BidId { get; set; }
        public string TenderTitle { get; set; }
        public string BuyerName { get; set; }
        public decimal ContractValue { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string Status { get; set; }
        public string PaymentTerms { get; set; }
        public string DeliverySchedule { get; set; }
    }
}
