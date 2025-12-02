namespace Wasel.ViewModels.SellerVMs.TicketVMs
{
    public class SellerTicketDetailsViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public string AssignedToName { get; set; }
        public string ReplyMessage { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ReplyUserName { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
