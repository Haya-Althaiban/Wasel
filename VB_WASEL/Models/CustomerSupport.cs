using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class CustomerSupport
{
    public int CsMemberId { get; set; }

    public string? CsMemberFirstname { get; set; }

    public string? CsMemberLastname { get; set; }

    public string? CsMemberPhone { get; set; }

    public string? CsMemberEmail { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
