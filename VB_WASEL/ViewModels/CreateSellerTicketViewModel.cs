using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.SellerVMs.TicketVMs
{
    public class CreateSellerTicketViewModel
    {
        [Required(ErrorMessage = "Issue type is required")]
        [StringLength(100, ErrorMessage = "Issue type cannot exceed 100 characters")]
        [Display(Name = "Issue Type")]
        public string TicketIssueType { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Describe your issue")]
        public string TicketDescription { get; set; }

        public string[] IssueTypes { get; set; }
    }
}
