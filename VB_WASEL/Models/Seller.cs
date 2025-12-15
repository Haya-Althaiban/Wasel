using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Seller
{
    public int SellerId { get; set; }

    public string SellerName { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public string? SellerCity { get; set; }

    public string? SellerAddress { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User? User { get; set; }
}
