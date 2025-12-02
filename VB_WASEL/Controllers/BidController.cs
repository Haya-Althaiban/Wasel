using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Models;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.BidVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class BidController : BaseController
    {
        public BidController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Seller's Bids

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
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                int pageSize = 10;

                // Get all bids for this seller
                var bidsQuery = _context.Bids
                    .Include(b => b.Tender)
                        .ThenInclude(t => t.Buyer)
                            .ThenInclude(b => b.User)
                    .Include(b => b.Tender)
                        .ThenInclude(t => t.Criteria)
                    .Include(b => b.Contracts)
                    .Where(b => b.SellerId == seller.SellerId)
                    .OrderByDescending(b => b.SubmissionDate);

                var totalItems = await bidsQuery.CountAsync();

                var bids = await bidsQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => new BidListItemViewModel
                    {
                        BidId = b.BidId,
                        TenderId = b.TenderId,
                        TenderTitle = b.Tender.TenderTitle,
                        BuyerName = b.Tender.Buyer.BuyerName,
                        ProposedPrice = b.ProposedPrice ?? 0,
                        ProposedTimeline = b.ProposedTimeline,
                        SubmissionDate = b.SubmissionDate,
                        IsApproved = b.IsApproved,
                        IsRejected = b.IsRejected,
                        HasContract = b.Contracts.Any(),
                        TenderStatus = b.Tender.TenderStatus,
                        TenderDeadline = b.Tender.SubmissionDeadline
                    })
                    .ToListAsync();

                // Calculate bid statistics
                var bidStats = await CalculateBidStatsAsync(seller.SellerId);

                var model = new BidIndexViewModel
                {
                    Bids = bids,
                    BidStats = bidStats,
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
                SetErrorMessage("An error occurred while loading bids.");
                return View(new BidIndexViewModel());
            }
        }

        #endregion

        #region Store/Update Bid

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(CreateBidViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please fill in all required fields correctly.");
                return RedirectToAction("Show", "Tender", new { id = model.TenderId });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = CurrentUser;
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (seller == null)
                {
                    SetErrorMessage("Seller profile not found.");
                    return RedirectToAction("Index", "Tender");
                }

                // Check if tender exists and is still open
                var tender = await _context.Tenders
                    .FirstOrDefaultAsync(t => t.TenderId == model.TenderId
                        && t.TenderStatus == "Open"
                        && t.SubmissionDeadline >= DateOnly.FromDateTime(DateTime.Now));

                if (tender == null)
                {
                    SetErrorMessage("This tender is no longer accepting bids.");
                    return RedirectToAction("Index", "Tender");
                }

                // Check if seller has already bid on this tender
                var existingBid = await _context.Bids
                    .FirstOrDefaultAsync(b => b.TenderId == model.TenderId
                        && b.SellerId == seller.SellerId);

                if (existingBid != null)
                {
                    // Update existing bid
                    existingBid.ProposedPrice = model.ProposedPrice;
                    existingBid.ProposedTimeline = model.ProposedTimeline;
                    existingBid.BidDescription = model.BidDescription;
                    existingBid.SubmissionDate = DateOnly.FromDateTime(DateTime.Now);

                    _context.Bids.Update(existingBid);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    SetSuccessMessage("Bid updated successfully!");
                    return RedirectToAction("Show", new { id = existingBid.BidId });
                }
                else
                {
                    // Create new bid
                    var bid = new Bid
                    {
                        SellerId = seller.SellerId,
                        TenderId = model.TenderId,
                        SubmissionDate = DateOnly.FromDateTime(DateTime.Now),
                        ProposedPrice = model.ProposedPrice,
                        ProposedTimeline = model.ProposedTimeline,
                        BidDescription = model.BidDescription,
                        IsApproved = false,
                        IsRejected = false
                    };

                    _context.Bids.Add(bid);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    SetSuccessMessage("Bid submitted successfully!");
                    return RedirectToAction("Show", new { id = bid.BidId });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError(ex);
                SetErrorMessage($"Failed to submit bid: {ex.Message}");
                return RedirectToAction("Show", "Tender", new { id = model.TenderId });
            }
        }

        #endregion

        #region Show Bid Details

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
                    return RedirectToAction("Index", "Home", new { area = "" });
                }

                var bid = await _context.Bids
                    .Include(b => b.Tender)
                        .ThenInclude(t => t.Buyer)
                            .ThenInclude(b => b.User)
                    .Include(b => b.Tender)
                        .ThenInclude(t => t.Criteria)
                    .Include(b => b.Contracts)
                        .ThenInclude(c => c.Payments)
                    .FirstOrDefaultAsync(b => b.BidId == id);

                if (bid == null)
                {
                    SetErrorMessage("Bid not found.");
                    return RedirectToAction("Index");
                }

                // Check if bid belongs to the authenticated seller
                if (bid.SellerId != seller.SellerId)
                {
                    SetErrorMessage("Unauthorized action.");
                    return RedirectToAction("Index");
                }

                // Calculate bid statistics for comparison
                var tenderBids = await _context.Bids
                    .Where(b => b.TenderId == bid.TenderId && b.BidId != bid.BidId)
                    .ToListAsync();

                var bidComparison = new BidComparisonViewModel
                {
                    TotalBids = tenderBids.Count + 1,
                    LowestBid = tenderBids.Any()
                        ? Math.Min(tenderBids.Min(b => b.ProposedPrice ?? 0), bid.ProposedPrice ?? 0)
                        : bid.ProposedPrice ?? 0,
                    AverageBid = tenderBids.Any()
                        ? (tenderBids.Average(b => b.ProposedPrice ?? 0) + (bid.ProposedPrice ?? 0)) / 2
                        : bid.ProposedPrice ?? 0,
                    HighestBid = tenderBids.Any()
                        ? Math.Max(tenderBids.Max(b => b.ProposedPrice ?? 0), bid.ProposedPrice ?? 0)
                        : bid.ProposedPrice ?? 0
                };

                // Calculate days since submission
                var daysSinceSubmission = bid.SubmissionDate.HasValue
                    ? (DateTime.Now - bid.SubmissionDate.Value.ToDateTime(TimeOnly.MinValue)).Days
                    : 0;

                var model = new BidDetailsViewModel
                {
                    BidId = bid.BidId,
                    TenderId = bid.TenderId,
                    TenderTitle = bid.Tender.TenderTitle,
                    TenderDescription = bid.Tender.TenderDescription,
                    TenderBudget = bid.Tender.TenderBudget ?? 0,
                    TenderStatus = bid.Tender.TenderStatus,
                    SubmissionDeadline = bid.Tender.SubmissionDeadline,
                    BuyerName = bid.Tender.Buyer.BuyerName,
                    BuyerEmail = bid.Tender.Buyer.User.Email,
                    BuyerPhone = bid.Tender.Buyer.ContactPhone,
                    ProposedPrice = bid.ProposedPrice ?? 0,
                    ProposedTimeline = bid.ProposedTimeline,
                    BidDescription = bid.BidDescription,
                    SubmissionDate = bid.SubmissionDate,
                    IsApproved = bid.IsApproved,
                    ApprovedAt = bid.ApprovedAt,
                    IsRejected = bid.IsRejected,
                    RejectedAt = bid.RejectedAt,
                    HasContract = bid.Contracts.Any(),
                    ContractId = bid.Contracts.FirstOrDefault()?.ContractId,
                    Criteria = bid.Tender.Criteria.Select(c => new CriteriaViewModel
                    {
                        CriteriaName = c.CriteriaName,
                        CriteriaDescription = c.CriteriaDescription,
                        Weight = c.Weight ?? 0,
                        DeliveryTime = c.DeliveryTime
                    }).ToList(),
                    BidComparison = bidComparison,
                    DaysSinceSubmission = daysSinceSubmission
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading bid details.");
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<BidStatsViewModel> CalculateBidStatsAsync(int sellerId)
        {
            var totalBids = await _context.Bids.CountAsync(b => b.SellerId == sellerId);

            var activeBids = await _context.Bids
                .Include(b => b.Tender)
                .Where(b => b.SellerId == sellerId
                    && !b.Contracts.Any()
                    && b.Tender.TenderStatus == "Open"
                    && b.Tender.SubmissionDeadline >= DateOnly.FromDateTime(DateTime.Now))
                .CountAsync();

            var awardedBids = await _context.Bids
                .Where(b => b.SellerId == sellerId && b.Contracts.Any())
                .CountAsync();

            var successRate = totalBids > 0 ? ((double)awardedBids / totalBids) * 100 : 0;

            // Recent activity
            var recentActivity = await _context.Bids
                .Include(b => b.Tender)
                .Where(b => b.SellerId == sellerId)
                .OrderByDescending(b => b.SubmissionDate)
                .Take(3)
                .Select(b => new RecentBidActivityViewModel
                {
                    BidId = b.BidId,
                    TenderTitle = b.Tender.TenderTitle,
                    SubmissionDate = b.SubmissionDate,
                    ProposedPrice = b.ProposedPrice ?? 0,
                    Status = b.IsApproved ? "Approved" : b.IsRejected ? "Rejected" : "Pending"
                })
                .ToListAsync();

            return new BidStatsViewModel
            {
                TotalBids = totalBids,
                ActiveBids = activeBids,
                AwardedBids = awardedBids,
                SuccessRate = Math.Round(successRate, 1),
                RecentActivity = recentActivity
            };
        }

        #endregion
    }
}