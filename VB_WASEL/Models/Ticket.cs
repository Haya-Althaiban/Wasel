using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Ticket
{
    public int TicketNum { get; set; }

    public int UserId { get; set; }

    public string? TicketIssueType { get; set; }

    public DateOnly? TicketOpenDate { get; set; }

    public string? TicketDescription { get; set; }

    public string? TicketStatus { get; set; }

    public DateOnly? TicketClosedDate { get; set; }

    public int? CsMemberId { get; set; }

    public string? ReplyMessage { get; set; }

    public DateTime? ReplyDate { get; set; }

    public int? ReplyUserId { get; set; }

    public virtual CustomerSupport? CsMember { get; set; }

    public virtual User? ReplyUser { get; set; }

    public virtual User User { get; set; } = null!;
}
