namespace Wasel.ViewModels.SellerVMs.FeedbackVMs
{
    public class FeedbackItemViewModel
    {
        public int FeedbackNum { get; set; }
        public string TenderTitle { get; set; }
        public string BuyerName { get; set; }
        public int? Rating { get; set; }
        public string Comment { get; set; }
        public DateOnly? FeedbackDate { get; set; }
    }
}
