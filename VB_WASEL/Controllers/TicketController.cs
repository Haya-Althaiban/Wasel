using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.TicketVMs;

namespace Wasel.Areas.Seller.Controllers
{
    
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class TicketController : BaseController
    {
        public TicketController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Tickets

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = CurrentUserId.Value;

                var tickets = await _context.Tickets
                    .Include(t => t.CsMember)
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.TicketOpenDate)
                    .Select(t => new SellerTicketListItemViewModel
                    {
                        TicketNum = t.TicketNum,
                        UserId = t.UserId,
                        TicketIssueType = t.TicketIssueType,
                        TicketDescription = t.TicketDescription.Length > 100
                            ? t.TicketDescription.Substring(0, 100) + "..."
                            : t.TicketDescription,
                        TicketStatus = t.TicketStatus,
                        TicketOpenDate = t.TicketOpenDate,
                        TicketClosedDate = t.TicketClosedDate,
                        HasReply = !string.IsNullOrEmpty(t.ReplyMessage),
                        AssignedToName = t.CsMember != null
                            ? $"{t.CsMember.CsMemberFirstname} {t.CsMember.CsMemberLastname}"
                            : null
                    })
                    .ToListAsync();

                var model = new SellerTicketIndexViewModel
                {
                    Tickets = tickets
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading tickets.");
                return View(new SellerTicketIndexViewModel());
            }
        }

        #endregion

        #region Create Ticket

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSellerTicketViewModel
            {
                IssueTypes = new[]
                {
                    "Technical Issue",
                    "Payment Problem",
                    "Account Issue",
                    "Tender/Bid Related",
                    "Contract Issue",
                    "Feedback/Review",
                    "Other"
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSellerTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IssueTypes = new[]
                {
                    "Technical Issue",
                    "Payment Problem",
                    "Account Issue",
                    "Tender/Bid Related",
                    "Contract Issue",
                    "Feedback/Review",
                    "Other"
                };
                return View(model);
            }

            try
            {
                var ticket = new Models.Ticket
                {
                    UserId = CurrentUserId.Value,
                    TicketIssueType = model.TicketIssueType,
                    TicketDescription = model.TicketDescription,
                    TicketStatus = "Open",
                    TicketOpenDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Support ticket created.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to create ticket. Please try again.");

                model.IssueTypes = new[]
                {
                    "Technical Issue",
                    "Payment Problem",
                    "Account Issue",
                    "Tender/Bid Related",
                    "Contract Issue",
                    "Feedback/Review",
                    "Other"
                };
                return View(model);
            }
        }

        #endregion

        #region Show Ticket

        [HttpGet]
        public async Task<IActionResult> Show(int ticketNum, int userId)
        {
            try
            {
                // Verify ticket belongs to current user
                if (userId != CurrentUserId.Value)
                {
                    SetErrorMessage("Unauthorized access.");
                    return RedirectToAction("Index");
                }

                var ticket = await _context.Tickets
                    .Include(t => t.CsMember)
                    .Include(t => t.User)
                    .Include(t => t.ReplyUser)
                    .FirstOrDefaultAsync(t => t.TicketNum == ticketNum && t.UserId == userId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                var model = new SellerTicketDetailsViewModel
                {
                    TicketNum = ticket.TicketNum,
                    UserId = ticket.UserId,
                    TicketIssueType = ticket.TicketIssueType,
                    TicketDescription = ticket.TicketDescription,
                    TicketStatus = ticket.TicketStatus,
                    TicketOpenDate = ticket.TicketOpenDate,
                    TicketClosedDate = ticket.TicketClosedDate,
                    AssignedToName = ticket.CsMember != null
                        ? $"{ticket.CsMember.CsMemberFirstname} {ticket.CsMember.CsMemberLastname}"
                        : null,
                    ReplyMessage = ticket.ReplyMessage,
                    ReplyDate = ticket.ReplyDate,
                    ReplyUserName = ticket.ReplyUser?.UserName,
                    CanEdit = ticket.TicketStatus == "Open",
                    CanDelete = ticket.TicketStatus == "Open"
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

        #region Edit Ticket

        [HttpGet]
        public async Task<IActionResult> Edit(int ticketNum, int userId)
        {
            try
            {
                // Verify ticket belongs to current user
                if (userId != CurrentUserId.Value)
                {
                    SetErrorMessage("Unauthorized access.");
                    return RedirectToAction("Index");
                }

                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == ticketNum && t.UserId == userId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                if (ticket.TicketStatus != "Open")
                {
                    SetErrorMessage("Only open tickets can be edited.");
                    return RedirectToAction("Show", new { ticketNum, userId });
                }

                var model = new EditSellerTicketViewModel
                {
                    TicketNum = ticket.TicketNum,
                    UserId = ticket.UserId,
                    TicketIssueType = ticket.TicketIssueType,
                    TicketDescription = ticket.TicketDescription,
                    IssueTypes = new[]
                    {
                        "Technical Issue",
                        "Payment Problem",
                        "Account Issue",
                        "Tender/Bid Related",
                        "Contract Issue",
                        "Feedback/Review",
                        "Other"
                    }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading ticket.");
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditSellerTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IssueTypes = new[]
                {
                    "Technical Issue",
                    "Payment Problem",
                    "Account Issue",
                    "Tender/Bid Related",
                    "Contract Issue",
                    "Feedback/Review",
                    "Other"
                };
                return View(model);
            }

            try
            {
                // Verify ticket belongs to current user
                if (model.UserId != CurrentUserId.Value)
                {
                    SetErrorMessage("Unauthorized access.");
                    return RedirectToAction("Index");
                }

                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == model.TicketNum && t.UserId == model.UserId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                ticket.TicketIssueType = model.TicketIssueType;
                ticket.TicketDescription = model.TicketDescription;

                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Ticket updated successfully.");
                return RedirectToAction("Show", new { ticketNum = model.TicketNum, userId = model.UserId });
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to update ticket. Please try again.");

                model.IssueTypes = new[]
                {
                    "Technical Issue",
                    "Payment Problem",
                    "Account Issue",
                    "Tender/Bid Related",
                    "Contract Issue",
                    "Feedback/Review",
                    "Other"
                };
                return View(model);
            }
        }

        #endregion

        #region Delete Ticket

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int ticketNum, int userId)
        {
            try
            {
                // Verify ticket belongs to current user
                if (userId != CurrentUserId.Value)
                {
                    SetErrorMessage("Unauthorized access.");
                    return RedirectToAction("Index");
                }

                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketNum == ticketNum && t.UserId == userId);

                if (ticket == null)
                {
                    SetErrorMessage("Ticket not found.");
                    return RedirectToAction("Index");
                }

                if (ticket.TicketStatus != "Open")
                {
                    SetErrorMessage("Only open tickets can be deleted.");
                    return RedirectToAction("Show", new { ticketNum, userId });
                }

                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Ticket deleted.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to delete ticket. Please try again.");
                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}