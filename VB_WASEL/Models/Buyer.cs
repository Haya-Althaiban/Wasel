using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Buyer
{
    public int BuyerId { get; set; }

    public string BuyerName { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public string? BuyerCity { get; set; }

    public string? BuyerAddress { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Tender> Tenders { get; set; } = new List<Tender>();

    public virtual User? User { get; set; }
}
