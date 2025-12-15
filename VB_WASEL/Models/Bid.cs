using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Bid
{
    public int BidId { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public decimal? ProposedPrice { get; set; }

    public string? ProposedTimeline { get; set; }

    public string? BidDescription { get; set; }

    public int SellerId { get; set; }

    public int TenderId { get; set; }

    public bool IsApproved { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public bool IsRejected { get; set; }

    public DateTime? RejectedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Seller Seller { get; set; } = null!;

    public virtual Tender Tender { get; set; } = null!;
}
