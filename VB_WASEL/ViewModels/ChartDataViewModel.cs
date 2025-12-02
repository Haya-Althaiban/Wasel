namespace Wasel.ViewModels.SellerVMs.DashboardVMs
{
    public class ChartDataViewModel
    {
        public List<int> Bids { get; set; } = new List<int>();
        public List<int> Contracts { get; set; } = new List<int>();
        public List<decimal> Revenue { get; set; } = new List<decimal>();
        public List<string> Months { get; set; } = new List<string>();
    }
}
