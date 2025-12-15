using System;
using System.Collections.Generic;

namespace Wasel.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public virtual ICollection<Buyer> Buyers { get; set; } = new List<Buyer>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageUsers { get; set; } = new List<Message>();

    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();

    public virtual ICollection<Ticket> TicketReplyUsers { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketUsers { get; set; } = new List<Ticket>();
}
