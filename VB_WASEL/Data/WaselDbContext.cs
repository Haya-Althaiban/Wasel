using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Wasel.Models;

namespace Wasel.Data;

public partial class WaselDbContext : DbContext
{
    public WaselDbContext()
    {
    }

    public WaselDbContext(DbContextOptions<WaselDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bid> Bids { get; set; }

    public virtual DbSet<Buyer> Buyers { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Criterion> Criteria { get; set; }

    public virtual DbSet<CustomerSupport> CustomerSupports { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<Tender> Tenders { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(e => e.BidId).HasName("PK__Bid__3F12797BAE9BB7BF");

            entity.ToTable("Bid");

            entity.Property(e => e.BidId).HasColumnName("BID_ID");
            entity.Property(e => e.ApprovedAt)
                .HasColumnType("datetime")
                .HasColumnName("approved_at");
            entity.Property(e => e.BidDescription).HasColumnName("BID_DESCRIPTION");
            entity.Property(e => e.IsApproved).HasColumnName("is_approved");
            entity.Property(e => e.IsRejected).HasColumnName("is_rejected");
            entity.Property(e => e.ProposedPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PROPOSED_PRICE");
            entity.Property(e => e.ProposedTimeline)
                .HasMaxLength(100)
                .HasColumnName("PROPOSED_TIMELINE");
            entity.Property(e => e.RejectedAt)
                .HasColumnType("datetime")
                .HasColumnName("rejected_at");
            entity.Property(e => e.SellerId).HasColumnName("SELLER_ID");
            entity.Property(e => e.SubmissionDate).HasColumnName("SUBMISSION_DATE");
            entity.Property(e => e.TenderId).HasColumnName("TENDER_ID");

            entity.HasOne(d => d.Seller).WithMany(p => p.Bids)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bid__SELLER_ID__5535A963");

            entity.HasOne(d => d.Tender).WithMany(p => p.Bids)
                .HasForeignKey(d => d.TenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bid__TENDER_ID__5629CD9C");
        });

        modelBuilder.Entity<Buyer>(entity =>
        {
            entity.HasKey(e => e.BuyerId).HasName("PK__Buyer__41FBBC49261798B5");

            entity.ToTable("Buyer");

            entity.Property(e => e.BuyerId).HasColumnName("BUYER_ID");
            entity.Property(e => e.BuyerAddress)
                .HasMaxLength(200)
                .HasColumnName("BUYER_ADDRESS");
            entity.Property(e => e.BuyerCity)
                .HasMaxLength(100)
                .HasColumnName("BUYER_CITY");
            entity.Property(e => e.BuyerName)
                .HasMaxLength(100)
                .HasColumnName("BUYER_NAME");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .HasColumnName("CONTACT_PHONE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Buyers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Buyer_User");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__3F5DFF14D42DE497");

            entity.ToTable("Contract");

            entity.Property(e => e.ContractId).HasColumnName("CONTRACT_ID");
            entity.Property(e => e.BidId).HasColumnName("BID_ID");
            entity.Property(e => e.ContractDocumentUrl)
                .HasMaxLength(255)
                .HasColumnName("CONTRACT_DOCUMENT_URL");
            entity.Property(e => e.ContractValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CONTRACT_VALUE");
            entity.Property(e => e.DeliverySchedule)
                .HasMaxLength(200)
                .HasColumnName("DELIVERY_SCHEDULE");
            entity.Property(e => e.EndDate).HasColumnName("END_DATE");
            entity.Property(e => e.PaymentTerms)
                .HasMaxLength(200)
                .HasColumnName("PAYMENT_TERMS");
            entity.Property(e => e.StartDate).HasColumnName("START_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Bid).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.BidId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contract__BID_ID__5812160E");
        });

        modelBuilder.Entity<Criterion>(entity =>
        {
            entity.HasKey(e => new { e.CriteriaNum, e.TenderId }).HasName("PK__Criteria__B0A40789F248448B");

            entity.Property(e => e.CriteriaNum)
                .ValueGeneratedOnAdd()
                .HasColumnName("CRITERIA_NUM");
            entity.Property(e => e.TenderId).HasColumnName("TENDER_ID");
            entity.Property(e => e.CriteriaDescription)
                .HasMaxLength(255)
                .HasColumnName("CRITERIA_DESCRIPTION");
            entity.Property(e => e.CriteriaName)
                .HasMaxLength(100)
                .HasColumnName("CRITERIA_NAME");
            entity.Property(e => e.DeliveryTime)
                .HasMaxLength(100)
                .HasColumnName("DELIVERY_TIME");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("WEIGHT");

            entity.HasOne(d => d.Tender).WithMany(p => p.Criteria)
                .HasForeignKey(d => d.TenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Criteria__TENDER__59063A47");
        });

        modelBuilder.Entity<CustomerSupport>(entity =>
        {
            entity.HasKey(e => e.CsMemberId).HasName("PK__Customer__EA82B17D89228D8D");

            entity.ToTable("CustomerSupport");

            entity.HasIndex(e => e.CsMemberEmail, "UQ__Customer__C7576FA34E39E4C6").IsUnique();

            entity.Property(e => e.CsMemberId).HasColumnName("CS_MEMBER_ID");
            entity.Property(e => e.CsMemberEmail)
                .HasMaxLength(150)
                .HasColumnName("CS_MEMBER_EMAIL");
            entity.Property(e => e.CsMemberFirstname)
                .HasMaxLength(100)
                .HasColumnName("CS_MEMBER_FIRSTNAME");
            entity.Property(e => e.CsMemberLastname)
                .HasMaxLength(100)
                .HasColumnName("CS_MEMBER_LASTNAME");
            entity.Property(e => e.CsMemberPhone)
                .HasMaxLength(20)
                .HasColumnName("CS_MEMBER_PHONE");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackNum).HasName("PK__Feedback__5D1621C102FB66A5");

            entity.ToTable("Feedback");

            entity.Property(e => e.FeedbackNum).HasColumnName("FEEDBACK_NUM");
            entity.Property(e => e.BuyerId).HasColumnName("BUYER_ID");
            entity.Property(e => e.Comment).HasColumnName("COMMENT");
            entity.Property(e => e.FeedbackDate).HasColumnName("FEEDBACK_DATE");
            entity.Property(e => e.Rating).HasColumnName("RATING");
            entity.Property(e => e.SellerId).HasColumnName("SELLER_ID");
            entity.Property(e => e.TenderId).HasColumnName("TENDER_ID");

            entity.HasOne(d => d.Buyer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__BUYER___59FA5E80");

            entity.HasOne(d => d.Seller).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__SELLER__5AEE82B9");

            entity.HasOne(d => d.Tender).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.TenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Feedback__TENDER__5BE2A6F2");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceNum).HasName("PK__Invoice__D753D683E2B3CC91");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceNum).HasColumnName("INVOICE_NUM");
            entity.Property(e => e.InvoiceDate).HasColumnName("INVOICE_DATE");
            entity.Property(e => e.InvoiceTime).HasColumnName("INVOICE_TIME");
            entity.Property(e => e.PaymentNum).HasColumnName("PAYMENT_NUM");

            entity.HasOne(d => d.PaymentNumNavigation).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.PaymentNum)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Invoice__PAYMENT__5CD6CB2B");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => new { e.MessageNum, e.UserId }).HasName("PK__Message__991CBF7F811ED7ED");

            entity.ToTable("Message");

            entity.Property(e => e.MessageNum)
                .ValueGeneratedOnAdd()
                .HasColumnName("MESSAGE_NUM");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.IsRead).HasColumnName("IS_READ");
            entity.Property(e => e.MessageText).HasColumnName("MESSAGE_TEXT");
            entity.Property(e => e.ReadAt)
                .HasColumnType("datetime")
                .HasColumnName("READ_AT");
            entity.Property(e => e.ReceiverId).HasColumnName("RECEIVER_ID");
            entity.Property(e => e.SenderId).HasColumnName("SENDER_ID");
            entity.Property(e => e.SentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("SENT_DATE");

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__RECEIVE__5DCAEF64");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__SENDER___5EBF139D");

            entity.HasOne(d => d.User).WithMany(p => p.MessageUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__USER_ID__5FB337D6");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentNum).HasName("PK__Payment__66424B426E1EBC91");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentNum).HasColumnName("PAYMENT_NUM");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.BuyerCommission)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("BUYER_COMMISSION");
            entity.Property(e => e.ContractId).HasColumnName("CONTRACT_ID");
            entity.Property(e => e.NetAmount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("NET_AMOUNT");
            entity.Property(e => e.PaymentDate).HasColumnName("PAYMENT_DATE");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(50)
                .HasColumnName("PAYMENT_STATUS");
            entity.Property(e => e.SellerCommission)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SELLER_COMMISSION");

            entity.HasOne(d => d.Contract).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__CONTRAC__60A75C0F");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__Seller__DE3D1B9992E2E823");

            entity.ToTable("Seller");

            entity.Property(e => e.SellerId).HasColumnName("SELLER_ID");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .HasColumnName("CONTACT_PHONE");
            entity.Property(e => e.SellerAddress)
                .HasMaxLength(200)
                .HasColumnName("SELLER_ADDRESS");
            entity.Property(e => e.SellerCity)
                .HasMaxLength(100)
                .HasColumnName("SELLER_CITY");
            entity.Property(e => e.SellerName)
                .HasMaxLength(100)
                .HasColumnName("SELLER_NAME");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Sellers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Seller_User");
        });

        modelBuilder.Entity<Tender>(entity =>
        {
            entity.HasKey(e => e.TenderId).HasName("PK__Tender__B11DE49FFC57AC2B");

            entity.ToTable("Tender");

            entity.Property(e => e.TenderId).HasColumnName("TENDER_ID");
            entity.Property(e => e.BuyerId).HasColumnName("BUYER_ID");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CREATED_DATE");
            entity.Property(e => e.PublishDate).HasColumnName("PUBLISH_DATE");
            entity.Property(e => e.SubmissionDeadline).HasColumnName("SUBMISSION_DEADLINE");
            entity.Property(e => e.TenderBudget)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TENDER_BUDGET");
            entity.Property(e => e.TenderDescription).HasColumnName("TENDER_DESCRIPTION");
            entity.Property(e => e.TenderStatus)
                .HasMaxLength(50)
                .HasColumnName("TENDER_STATUS");
            entity.Property(e => e.TenderTitle)
                .HasMaxLength(200)
                .HasColumnName("TENDER_TITLE");

            entity.HasOne(d => d.Buyer).WithMany(p => p.Tenders)
                .HasForeignKey(d => d.BuyerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tender__BUYER_ID__628FA481");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => new { e.TicketNum, e.UserId }).HasName("PK__Ticket__6E08B82BC45D59C7");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketNum)
                .ValueGeneratedOnAdd()
                .HasColumnName("TICKET_NUM");
            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.CsMemberId).HasColumnName("CS_MEMBER_ID");
            entity.Property(e => e.ReplyDate)
                .HasColumnType("datetime")
                .HasColumnName("REPLY_DATE");
            entity.Property(e => e.ReplyMessage).HasColumnName("REPLY_MESSAGE");
            entity.Property(e => e.ReplyUserId).HasColumnName("REPLY_USER_ID");
            entity.Property(e => e.TicketClosedDate).HasColumnName("TICKET_CLOSED_DATE");
            entity.Property(e => e.TicketDescription).HasColumnName("TICKET_DESCRIPTION");
            entity.Property(e => e.TicketIssueType)
                .HasMaxLength(100)
                .HasColumnName("TICKET_ISSUE_TYPE");
            entity.Property(e => e.TicketOpenDate).HasColumnName("TICKET_OPEN_DATE");
            entity.Property(e => e.TicketStatus)
                .HasMaxLength(50)
                .HasColumnName("TICKET_STATUS");

            entity.HasOne(d => d.CsMember).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.CsMemberId)
                .HasConstraintName("FK__Ticket__CS_MEMBE__6383C8BA");

            entity.HasOne(d => d.ReplyUser).WithMany(p => p.TicketReplyUsers)
                .HasForeignKey(d => d.ReplyUserId)
                .HasConstraintName("FK_Ticket_ReplyUser");

            entity.HasOne(d => d.User).WithMany(p => p.TicketUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__USER_ID__6477ECF3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__F3BEEBFF5183BDA7");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__161CF72480AD6934").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("USER_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .HasColumnName("USER_NAME");
            entity.Property(e => e.UserType)
                .HasMaxLength(50)
                .HasColumnName("USER_TYPE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
