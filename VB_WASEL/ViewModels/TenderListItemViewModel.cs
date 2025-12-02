namespace Wasel.ViewModels.SellerVMs.TenderVMs
{
    public class TenderListItemViewModel
    {
        public int TenderId { get; set; }
        public string TenderTitle { get; set; }
        public string TenderDescription { get; set; }
        public decimal TenderBudget { get; set; }
        public string TenderStatus { get; set; }
        public DateOnly? PublishDate { get; set; }
        public DateOnly? SubmissionDeadline { get; set; }
        public string BuyerName { get; set; }
        public string BuyerCity { get; set; }
        public int CriteriaCount { get; set; }
        public bool HasBid { get; set; }
        public int? BidId { get; set; }
        public int DaysRemaining { get; set; }
    }

}
