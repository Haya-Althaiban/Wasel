using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Criterion
{
    public int CriteriaNum { get; set; }

    public int TenderId { get; set; }

    public string? CriteriaName { get; set; }

    public string? CriteriaDescription { get; set; }

    public decimal? Weight { get; set; }

    public string? DeliveryTime { get; set; }

    public virtual Tender Tender { get; set; } = null!;
}
