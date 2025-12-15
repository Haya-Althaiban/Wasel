using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class Message
{
    public int MessageNum { get; set; }

    public int UserId { get; set; }

    public string? MessageText { get; set; }

    public DateTime? SentDate { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User Receiver { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
