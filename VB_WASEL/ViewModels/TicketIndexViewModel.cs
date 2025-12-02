using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.TicketVMs
{
    #region Ticket Index

    public class TicketIndexViewModel
    {
        public List<TicketListItemViewModel> Tickets { get; set; } = new List<TicketListItemViewModel>();
        public TicketStatsViewModel TicketStats { get; set; } = new TicketStatsViewModel();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class TicketListItemViewModel
    {
        public int TicketNum { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public bool HasReply { get; set; }
        public string AssignedToName { get; set; }
    }

    public class TicketStatsViewModel
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int Closed { get; set; }
        public int InProgress { get; set; }
    }

    #endregion

    #region Create Ticket

    public class CreateTicketViewModel
    {
        [Required(ErrorMessage = "Issue type is required")]
        [StringLength(100, ErrorMessage = "Issue type cannot exceed 100 characters")]
        [Display(Name = "Issue Type")]
        public string TicketIssueType { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
        [Display(Name = "Describe your issue")]
        public string TicketDescription { get; set; }

        public string[] IssueTypes { get; set; }
    }

    #endregion

    #region Ticket Details

    public class TicketDetailsViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string AssignedToName { get; set; }
        public string AssignedToEmail { get; set; }
        public string ReplyMessage { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ReplyUserName { get; set; }
        public bool CanClose { get; set; }
        public bool CanReopen { get; set; }
    }

    #endregion
}
