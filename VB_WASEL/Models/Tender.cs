using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Tender
{
    public int TenderId { get; set; }

    public string TenderTitle { get; set; } = null!;

    public decimal? TenderBudget { get; set; }

    public string? TenderDescription { get; set; }

    public string? TenderStatus { get; set; }

    public DateOnly? PublishDate { get; set; }

    public DateOnly? SubmissionDeadline { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int BuyerId { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual Buyer Buyer { get; set; } = null!;

    public virtual ICollection<Criterion> Criteria { get; set; } = new List<Criterion>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
