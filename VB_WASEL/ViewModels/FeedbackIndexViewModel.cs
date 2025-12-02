
namespace Wasel.ViewModels.SellerVMs.FeedbackVMs
{
    public class FeedbackIndexViewModel
    {
        public List<FeedbackItemViewModel> Feedbacks { get; set; } = new List<FeedbackItemViewModel>();
        public FeedbackStatsViewModel FeedbackStats { get; set; } = new FeedbackStatsViewModel();
        public List<TenderDropdownViewModel> SellerTenders { get; set; } = new List<TenderDropdownViewModel>();
        public string SelectedRating { get; set; }
        public int? SelectedTender { get; set; }
    }
}
