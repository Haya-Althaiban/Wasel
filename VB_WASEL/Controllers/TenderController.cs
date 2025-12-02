using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.TenderVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class TenderController : BaseController
    {
        public TenderController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Open Tenders

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var user = CurrentUser;
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (seller == null)
                {
                    SetErrorMessage("Seller profile not found.");
                    return RedirectToAction("Dashboard", "Seller");
                }

                int pageSize = 5;

                // Get open tenders with submission deadline in future
                var tendersQuery = _context.Tenders
                    .Include(t => t.Buyer)
                        .ThenInclude(b => b.User)
                    .Include(t => t.Criteria)
                    .Include(t => t.Bids.Where(b => b.SellerId == seller.SellerId))
                    .Where(t => t.TenderStatus == "Open"
                        && t.SubmissionDeadline >= DateOnly.FromDateTime(DateTime.Now))
                    .OrderBy(t => t.SubmissionDeadline);

                var totalItems = await tendersQuery.CountAsync();

                var tenders = await tendersQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TenderListItemViewModel
                    {
                        TenderId = t.TenderId,
                        TenderTitle = t.TenderTitle,
                        TenderDescription = t.TenderDescription.Length > 150
                            ? t.TenderDescription.Substring(0, 150) + "..."
                            : t.TenderDescription,
                        TenderBudget = t.TenderBudget ?? 0,
                        TenderStatus = t.TenderStatus,
                        PublishDate = t.PublishDate,
                        SubmissionDeadline = t.SubmissionDeadline,
                        BuyerName = t.Buyer.BuyerName,
                        BuyerCity = t.Buyer.BuyerCity,
                        CriteriaCount = t.Criteria.Count,
                        HasBid = t.Bids.Any(),
                        BidId = t.Bids.FirstOrDefault() != null ? t.Bids.FirstOrDefault().BidId : (int?)null,
                        DaysRemaining = t.SubmissionDeadline.HasValue
                            ? (t.SubmissionDeadline.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days
                            : 0
                    })
                    .ToListAsync();

                // Get tender statistics
                var tenderStats = await GetTenderStatsAsync();

                var model = new TenderIndexViewModel
                {
                    Tenders = tenders,
                    TenderStats = tenderStats,
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
                SetErrorMessage("An error occurred while loading tenders.");
                return View(new TenderIndexViewModel());
            }
        }

        #endregion

        #region Show Tender Details

        [HttpGet]
        public async Task<IActionResult> Show(int id)
        {
            try
            {
                var user = CurrentUser;
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (seller == null)
                {
                    SetErrorMessage("Seller profile not found.");
                    return RedirectToAction("Dashboard", "Seller");
                }

                var tender = await _context.Tenders
                    .Include(t => t.Buyer)
                        .ThenInclude(b => b.User)
                    .Include(t => t.Criteria)
                    .FirstOrDefaultAsync(t => t.TenderId == id);

                if (tender == null)
                {
                    SetErrorMessage("Tender not found.");
                    return RedirectToAction("Index");
                }

                // Check if tender is open and deadline hasn't passed
                if (tender.TenderStatus != "Open" ||
                    tender.SubmissionDeadline < DateOnly.FromDateTime(DateTime.Now))
                {
                    SetErrorMessage("This tender is no longer accepting bids.");
                    return RedirectToAction("Index");
                }

                // Check if seller has already bid on this tender
                var existingBid = await _context.Bids
                    .FirstOrDefaultAsync(b => b.TenderId == tender.TenderId
                        && b.SellerId == seller.SellerId);

                // Calculate days remaining
                var daysRemaining = tender.SubmissionDeadline.HasValue
                    ? (tender.SubmissionDeadline.Value.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days
                    : 0;

                var isDeadlineClose = daysRemaining <= 3;

                var model = new TenderDetailsViewModel
                {
                    TenderId = tender.TenderId,
                    TenderTitle = tender.TenderTitle,
                    TenderDescription = tender.TenderDescription,
                    TenderBudget = tender.TenderBudget ?? 0,
                    TenderStatus = tender.TenderStatus,
                    PublishDate = tender.PublishDate,
                    SubmissionDeadline = tender.SubmissionDeadline,
                    CreatedDate = tender.CreatedDate,
                    BuyerName = tender.Buyer.BuyerName,
                    BuyerEmail = tender.Buyer.User.Email,
                    BuyerPhone = tender.Buyer.ContactPhone,
                    BuyerCity = tender.Buyer.BuyerCity,
                    BuyerAddress = tender.Buyer.BuyerAddress,
                    Criteria = tender.Criteria.Select(c => new CriteriaViewModel
                    {
                        CriteriaNum = c.CriteriaNum,
                        CriteriaName = c.CriteriaName,
                        CriteriaDescription = c.CriteriaDescription,
                        Weight = c.Weight ?? 0,
                        DeliveryTime = c.DeliveryTime
                    }).ToList(),
                    ExistingBid = existingBid != null ? new ExistingBidViewModel
                    {
                        BidId = existingBid.BidId,
                        ProposedPrice = existingBid.ProposedPrice ?? 0,
                        ProposedTimeline = existingBid.ProposedTimeline,
                        BidDescription = existingBid.BidDescription,
                        SubmissionDate = existingBid.SubmissionDate
                    } : null,
                    DaysRemaining = daysRemaining,
                    IsDeadlineClose = isDeadlineClose
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading tender details.");
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<TenderStatsViewModel> GetTenderStatsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var threeDaysFromNow = DateOnly.FromDateTime(DateTime.Now.AddDays(3));

            var totalTenders = await _context.Tenders
                .CountAsync(t => t.TenderStatus == "Open"
                    && t.SubmissionDeadline >= today);

            var endingSoon = await _context.Tenders
                .CountAsync(t => t.TenderStatus == "Open"
                    && t.SubmissionDeadline <= threeDaysFromNow
                    && t.SubmissionDeadline >= today);

            var highValue = await _context.Tenders
                .CountAsync(t => t.TenderStatus == "Open"
                    && t.SubmissionDeadline >= today
                    && t.TenderBudget >= 100000);

            return new TenderStatsViewModel
            {
                TotalTenders = totalTenders,
                EndingSoon = endingSoon,
                HighValue = highValue
            };
        }

        #endregion
    }
}