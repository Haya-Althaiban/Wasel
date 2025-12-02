using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Data;
using Wasel.Models;
using Wasel.Services;
using Wasel.ViewModels.SupportVMs;

namespace Wasel.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class SupportController : BaseController
    {
        public SupportController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List All Tickets

        [HttpGet]
        public async Task<IActionResult> Index(string status = "all", int page = 1)
        {
            try
            {
                int pageSize = 20;

                var ticketsQuery = _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.CsMember)
                    .Include(t => t.ReplyUser)
                    .AsQueryable();

                if (status != "all")
                {
                    ticketsQuery = ticketsQuery.Where(t => t.TicketStatus == status);
                }

                ticketsQuery = ticketsQuery.OrderByDescending(t => t.TicketOpenDate);

                var totalItems = await ticketsQuery.CountAsync();

                var tickets = await ticketsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new SupportTicketListItemViewModel
                    {
                        TicketNum = t.TicketNum,
                        UserId = t.UserId,
                        UserName = t.User.UserName,
                        UserEmail = t.User.Email,
                        TicketIssueType = t.TicketIssueType,
                        TicketStatus = t.TicketStatus,
                        TicketOpenDate = t.TicketOpenDate,
                        TicketClosedDate = t.TicketClosedDate,
                        HasReply = !string.IsNullOrEmpty(t.ReplyMessage),
                        AssignedToName = t.CsMember != null
                            ? $"{t.CsMember.CsMemberFirstname} {t.CsMember.CsMemberLastname}"
                            : "Unassigned"
                    })
                    .ToListAsync();

                var stats = new SupportTicketStatsViewModel
                {
                    Total = await _context.Tickets.CountAsync(),
                    Open = await _context.Tickets.CountAsync(t => t.TicketStatus == "Open"),
                    InProgress = await _context.Tickets.CountAsync(t => t.TicketStatus == "In Progress"),
                    Closed = await _context.Tickets.CountAsync(t => t.TicketStatus == "Closed"),
                    Replied = await _context.Tickets.CountAsync(t => !string.IsNullOrEmpty(t.ReplyMessage))
                };

                var supportMembers = await _context.CustomerSupports
                    .Select(cs => new SupportMemberViewModel
                    {
                        CsMemberId = cs.CsMemberId,
                        FullName = $"{cs.CsMemberFirstname} {cs.CsMemberLastname}",
                        Email = cs.CsMemberEmail
                    })
                    .ToListAsync();

                var model = new SupportIndexViewModel
                {
                    Tickets = tickets,
                    TicketStats = stats,
                    SupportMembers = supportMembers,
                    CurrentStatus = status,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                    HasPreviousPage = page > 1,
                    HasNextPage = page < (int)Math.Ceiling(totalItems / (double)pageSize)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading tickets.");
                return View(new SupportIndexViewModel());
            }
        }

        #endregion

        #region Show Ticket Details

        [HttpGet]
        public async Task<IActionResult> Show(int ticketNum, int userId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.CsMember)
                    .Include(t => t.ReplyUser)
                    .FirstOrDefaultAsync(t => t.TicketNum == ticketNum && t.UserId == userId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                var supportMembers = await _context.CustomerSupports
                    .Select(cs => new SupportMemberViewModel
                    {
                        CsMemberId = cs.CsMemberId,
                        FullName = $"{cs.CsMemberFirstname} {cs.CsMemberLastname}",
                        Email = cs.CsMemberEmail
                    })
                    .ToListAsync();

                var model = new SupportTicketDetailsViewModel
                {
                    TicketNum = ticket.TicketNum,
                    UserId = ticket.UserId,
                    UserName = ticket.User.UserName,
                    UserEmail = ticket.User.Email,
                    TicketIssueType = ticket.TicketIssueType,
                    TicketDescription = ticket.TicketDescription,
                    TicketStatus = ticket.TicketStatus,
                    TicketOpenDate = ticket.TicketOpenDate,
                    TicketClosedDate = ticket.TicketClosedDate,
                    CsMemberId = ticket.CsMemberId,
                    AssignedToName = ticket.CsMember != null
                        ? $"{ticket.CsMember.CsMemberFirstname} {ticket.CsMember.CsMemberLastname}"
                        : null,
                    ReplyMessage = ticket.ReplyMessage,
                    ReplyDate = ticket.ReplyDate,
                    ReplyUserName = ticket.ReplyUser?.UserName,
                    SupportMembers = supportMembers
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading ticket details.");
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Assign Ticket

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(AssignTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Invalid assignment data.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }

            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == model.TicketNum && t.UserId == model.UserId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.CsMemberId = model.CsMemberId;
                ticket.TicketStatus = "In Progress";

                await _context.SaveChangesAsync();

                SetSuccessMessage("Ticket assigned successfully!");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to assign ticket. Please try again.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
        }

        #endregion

        #region Update Status

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateTicketStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Invalid status data.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }

            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == model.TicketNum && t.UserId == model.UserId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.TicketStatus = model.Status;

                if (model.Status == "Closed")
                {
                    ticket.TicketClosedDate = DateOnly.FromDateTime(DateTime.Now);
                }

                await _context.SaveChangesAsync();

                SetSuccessMessage("Ticket status updated successfully!");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to update ticket status. Please try again.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
        }

        #endregion

        #region Add Reply

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReply(AddTicketReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please provide a reply message.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }

            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == model.TicketNum && t.UserId == model.UserId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.ReplyMessage = model.Message;
                ticket.ReplyDate = DateTime.Now;
                ticket.ReplyUserId = CurrentUserId.Value;
                ticket.TicketStatus = "In Progress";

                await _context.SaveChangesAsync();

                SetSuccessMessage("Reply added successfully!");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to add reply. Please try again.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
        }

        #endregion

        #region Edit Reply

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReply(AddTicketReplyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please provide a reply message.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }

            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == model.TicketNum && t.UserId == model.UserId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.ReplyMessage = model.Message;
                ticket.ReplyDate = DateTime.Now;
                ticket.ReplyUserId = CurrentUserId.Value;

                await _context.SaveChangesAsync();

                SetSuccessMessage("Reply updated successfully!");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to update reply. Please try again.");
                return RedirectToAction("Show", new { model.TicketNum, model.UserId });
            }
        }

        #endregion

        #region Delete Reply

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReply(int ticketNum, int userId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == ticketNum && t.UserId == userId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.ReplyMessage = null;
                ticket.ReplyDate = null;
                ticket.ReplyUserId = null;

                await _context.SaveChangesAsync();

                SetSuccessMessage("Reply deleted successfully!");
                return RedirectToAction("Show", new { ticketNum, userId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to delete reply. Please try again.");
                return RedirectToAction("Show", new { ticketNum, userId });
            }
        }

        #endregion

        #region Statistics

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                var ticketsByType = await _context.Tickets
                    .GroupBy(t => t.TicketIssueType)
                    .Select(g => new TicketsByTypeViewModel
                    {
                        IssueType = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var recentTickets = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.ReplyUser)
                    .OrderByDescending(t => t.TicketOpenDate)
                    .Take(10)
                    .Select(t => new RecentTicketViewModel
                    {
                        TicketNum = t.TicketNum,
                        UserId = t.UserId,
                        UserName = t.User.UserName,
                        TicketIssueType = t.TicketIssueType,
                        TicketStatus = t.TicketStatus,
                        TicketOpenDate = t.TicketOpenDate,
                        HasReply = !string.IsNullOrEmpty(t.ReplyMessage)
                    })
                    .ToListAsync();

                var model = new SupportStatisticsViewModel
                {
                    TotalTickets = await _context.Tickets.CountAsync(),
                    OpenTickets = await _context.Tickets.CountAsync(t => t.TicketStatus == "Open"),
                    InProgressTickets = await _context.Tickets.CountAsync(t => t.TicketStatus == "In Progress"),
                    ClosedTickets = await _context.Tickets.CountAsync(t => t.TicketStatus == "Closed"),
                    RepliedTickets = await _context.Tickets.CountAsync(t => !string.IsNullOrEmpty(t.ReplyMessage)),
                    AverageResponseTime = await CalculateAverageResponseTimeAsync(),
                    TicketsByType = ticketsByType,
                    RecentTickets = recentTickets
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading statistics.");
                return View(new SupportStatisticsViewModel());
            }
        }

        private async Task<string> CalculateAverageResponseTimeAsync()
        {
            var ticketsWithReply = await _context.Tickets
                .Where(t => t.ReplyDate != null && t.TicketOpenDate != null)
                .ToListAsync();

            if (!ticketsWithReply.Any())
            {
                return "No replies yet";
            }

            double totalHours = 0;
            foreach (var ticket in ticketsWithReply)
            {
                var openDate = ticket.TicketOpenDate.Value.ToDateTime(TimeOnly.MinValue);
                var replyDate = ticket.ReplyDate.Value;
                totalHours += (replyDate - openDate).TotalHours;
            }

            double averageHours = totalHours / ticketsWithReply.Count;

            if (averageHours < 1)
            {
                return "Less than 1 hour";
            }
            else if (averageHours < 24)
            {
                return $"{Math.Round(averageHours)} hours";
            }
            else
            {
                return $"{Math.Round(averageHours / 24)} days";
            }
        }

        #endregion
    }
}