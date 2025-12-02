using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.SellerVMs.BidVMs
{
    public class CreateBidViewModel
    {
        [Required(ErrorMessage = "Tender ID is required")]
        public int TenderId { get; set; }

        [Required(ErrorMessage = "Proposed price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Proposed price must be a positive number")]
        [Display(Name = "Proposed Price")]
        public decimal ProposedPrice { get; set; }

        [Required(ErrorMessage = "Proposed timeline is required")]
        [StringLength(100, ErrorMessage = "Timeline cannot exceed 100 characters")]
        [Display(Name = "Proposed Timeline")]
        public string ProposedTimeline { get; set; }

        [Required(ErrorMessage = "Bid description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Bid Description")]
        public string BidDescription { get; set; }
    }
}
