using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public decimal? ContractValue { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? PaymentTerms { get; set; }

    public string? DeliverySchedule { get; set; }

    public string? Status { get; set; }

    public string? ContractDocumentUrl { get; set; }

    public int BidId { get; set; }

    public virtual Bid Bid { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
