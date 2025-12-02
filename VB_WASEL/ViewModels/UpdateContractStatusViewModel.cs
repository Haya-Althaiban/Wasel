using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.SellerVMs.ContractVMs
{
    public class UpdateContractStatusViewModel
    {
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        [RegularExpression("^(Approved|Rejected|Pending)$", ErrorMessage = "Invalid status")]
        [Display(Name = "Contract Status")]
        public string Status { get; set; }
    }
}
