using System.ComponentModel.DataAnnotations;

namespace Wasel.ViewModels.SupportVMs
{
    #region Support Index

    public class SupportIndexViewModel
    {
        public List<SupportTicketListItemViewModel> Tickets { get; set; } = new List<SupportTicketListItemViewModel>();
        public SupportTicketStatsViewModel TicketStats { get; set; } = new SupportTicketStatsViewModel();
        public List<SupportMemberViewModel> SupportMembers { get; set; } = new List<SupportMemberViewModel>();
        public string CurrentStatus { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    public class SupportTicketListItemViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public bool HasReply { get; set; }
        public string AssignedToName { get; set; }
    }

    public class SupportTicketStatsViewModel
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Closed { get; set; }
        public int Replied { get; set; }
    }

    public class SupportMemberViewModel
    {
        public int CsMemberId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }

    #endregion

    #region Ticket Details

    public class SupportTicketDetailsViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public DateOnly? TicketClosedDate { get; set; }
        public int? CsMemberId { get; set; }
        public string AssignedToName { get; set; }
        public string ReplyMessage { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ReplyUserName { get; set; }
        public List<SupportMemberViewModel> SupportMembers { get; set; } = new List<SupportMemberViewModel>();
    }

    #endregion

    #region Assign Ticket

    public class AssignTicketViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please select a support member")]
        [Display(Name = "Assign To")]
        public int CsMemberId { get; set; }
    }

    #endregion

    #region Update Status

    public class UpdateTicketStatusViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please select a status")]
        [RegularExpression("^(Open|In Progress|Closed)$", ErrorMessage = "Invalid status")]
        [Display(Name = "Status")]
        public string Status { get; set; }
    }

    #endregion

    #region Add/Edit Reply

    public class AddTicketReplyViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Reply message is required")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "Reply must be between 1 and 2000 characters")]
        [Display(Name = "Reply Message")]
        public string Message { get; set; }
    }

    #endregion

    #region Statistics

    public class SupportStatisticsViewModel
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int RepliedTickets { get; set; }
        public string AverageResponseTime { get; set; }
        public List<TicketsByTypeViewModel> TicketsByType { get; set; } = new List<TicketsByTypeViewModel>();
        public List<RecentTicketViewModel> RecentTickets { get; set; } = new List<RecentTicketViewModel>();
    }

    public class TicketsByTypeViewModel
    {
        public string IssueType { get; set; }
        public int Count { get; set; }
    }

    public class RecentTicketViewModel
    {
        public int TicketNum { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string TicketIssueType { get; set; }
        public string TicketStatus { get; set; }
        public DateOnly? TicketOpenDate { get; set; }
        public bool HasReply { get; set; }
    }

    #endregion
}
