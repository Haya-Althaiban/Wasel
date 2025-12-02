using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.SellerVMs.MessageVMs
{
    public class SendSellerMessageViewModel
    {
        [Required(ErrorMessage = "Receiver is required")]
        [Display(Name = "Send To")]
        public int ReceiverId { get; set; }

        [Required(ErrorMessage = "Message text is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        [Display(Name = "Message")]
        public string MessageText { get; set; }
    }
}
