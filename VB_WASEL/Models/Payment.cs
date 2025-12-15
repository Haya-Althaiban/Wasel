using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Payment
{
    public int PaymentNum { get; set; }

    public int ContractId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? BuyerCommission { get; set; }

    public decimal? SellerCommission { get; set; }

    public decimal? NetAmount { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Contract Contract { get; set; } = null!;

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
