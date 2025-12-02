
namespace Wasel.ViewModels.SellerVMs.PaymentVMs
{
    public class PaymentIndexViewModel
    {
        public List<PaymentListItemViewModel> Payments { get; set; } = new List<PaymentListItemViewModel>();
        public PaymentStatsViewModel PaymentStats { get; set; } = new PaymentStatsViewModel();
    }
}
