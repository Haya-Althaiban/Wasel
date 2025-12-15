using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Feedback
{
    public int FeedbackNum { get; set; }

    public string? Comment { get; set; }

    public DateOnly? FeedbackDate { get; set; }

    public int SellerId { get; set; }

    public int BuyerId { get; set; }

    public int TenderId { get; set; }

    public int? Rating { get; set; }

    public virtual Buyer Buyer { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;

    public virtual Tender Tender { get; set; } = null!;
}
