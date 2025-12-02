using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.BuyerVMs
{
    public class BuyerDashboardViewModel
    {
        public Models.Buyer Buyer { get; set; }
        public int ActiveTenders { get; set; }
        public int TotalBidsReceived { get; set; }
        public int TotalTenders { get; set; }
        public int ClosedTenders { get; set; }
        public List<RecentTenderViewModel> RecentTenders { get; set; } = new List<RecentTenderViewModel>();
        public ChartDataViewModel ChartData { get; set; } = new ChartDataViewModel();
    }

    public class RecentTenderViewModel
    {
        public int TenderId { get; set; }
        public string? TenderTitle { get; set; }
        public string? TenderStatus { get; set; }
        public DateOnly? PublishDate { get; set; }
        public DateOnly? SubmissionDeadline { get; set; }
        public decimal? TenderBudget { get; set; }
        public int BidsCount { get; set; }

        public string StatusBadgeClass => TenderStatus switch
        {
            "Open" => "badge bg-success",
            "Closed" => "badge bg-secondary",
            "Cancelled" => "badge bg-danger",
            _ => "badge bg-info"
        };

        public string StatusDisplayText => TenderStatus switch
        {
            "Open" => "مفتوحة",
            "Closed" => "مغلقة",
            "Cancelled" => "ملغاة",
            _ => TenderStatus ?? "غير محدد"
        };
    }

    public class ChartDataViewModel
    {
        public List<string> Months { get; set; } = new List<string>();
        public List<int> Tenders { get; set; } = new List<int>();
        public List<int> Bids { get; set; } = new List<int>();
        public List<int> Contracts { get; set; } = new List<int>();
    }

    public class BuyerProfileViewModel
    {
        public int BuyerId { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف")]
        [Display(Name = "الاسم")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [StringLength(150, ErrorMessage = "البريد الإلكتروني يجب ألا يتجاوز 150 حرف")]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [StringLength(20, ErrorMessage = "رقم الهاتف يجب ألا يتجاوز 20 حرف")]
        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [StringLength(100, ErrorMessage = "المدينة يجب ألا تتجاوز 100 حرف")]
        [Display(Name = "المدينة")]
        public string? City { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(200, ErrorMessage = "العنوان يجب ألا يتجاوز 200 حرف")]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }
    }
}
