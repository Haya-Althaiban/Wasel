namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class BidComparisonViewModel
    {
        public int TotalBids { get; set; }
        public decimal LowestBid { get; set; }
        public decimal AverageBid { get; set; }
        public decimal HighestBid { get; set; }
    }
}
