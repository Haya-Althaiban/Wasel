namespace Wasel.ViewModels.SellerVMs.TicketVMs
{
    public class SellerTicketListItemViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public bool HasReply { get; set; }
        public string AssignedToName { get; set; }
    }
}
