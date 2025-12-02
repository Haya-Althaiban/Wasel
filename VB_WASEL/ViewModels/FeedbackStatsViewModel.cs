namespace Wasel.ViewModels.SellerVMs.FeedbackVMs
{
    public class FeedbackStatsViewModel
    {
        public int Total { get; set; }
        public int WithComments { get; set; }
        public int Recent { get; set; }
        public int PositiveFeedback { get; set; }
        public double AverageRating { get; set; }
    }
}
