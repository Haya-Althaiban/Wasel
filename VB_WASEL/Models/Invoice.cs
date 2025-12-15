using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Invoice
{
    public int InvoiceNum { get; set; }

    public TimeOnly? InvoiceTime { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public int PaymentNum { get; set; }

    public virtual Payment PaymentNumNavigation { get; set; } = null!;
}
