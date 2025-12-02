using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.FeedbackVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class FeedbackController : BaseController
    {
        public FeedbackController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Feedback

        [HttpGet]
        public async Task<IActionResult> Index(string rating = null, int? tender = null)
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

                // Get feedbacks with filters
                var feedbacksQuery = _context.Feedbacks
                    .Include(f => f.Tender)
                    .Include(f => f.Buyer)
                        .ThenInclude(b => b.User)
                    .Where(f => f.SellerId == seller.SellerId);

                // Apply rating filter
                if (!string.IsNullOrEmpty(rating))
                {
                    if (rating == "with_comment")
                    {
                        feedbacksQuery = feedbacksQuery.Where(f => !string.IsNullOrEmpty(f.Comment));
                    }
                    else if (rating == "without_comment")
                    {
                        feedbacksQuery = feedbacksQuery.Where(f => string.IsNullOrEmpty(f.Comment));
                    }
                    else if (int.TryParse(rating, out int ratingValue))
                    {
                        feedbacksQuery = feedbacksQuery.Where(f => f.Rating == ratingValue);
                    }
                }

                // Apply tender filter
                if (tender.HasValue)
                {
                    feedbacksQuery = feedbacksQuery.Where(f => f.TenderId == tender.Value);
                }

                var feedbacks = await feedbacksQuery
                    .OrderByDescending(f => f.FeedbackDate)
                    .Select(f => new FeedbackItemViewModel
                    {
                        FeedbackNum = f.FeedbackNum,
                        TenderTitle = f.Tender.TenderTitle,
                        BuyerName = f.Buyer.BuyerName,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        FeedbackDate = f.FeedbackDate
                    })
                    .ToListAsync();

                // Get tenders with feedback for filter dropdown
                var sellerTenders = await _context.Tenders
                    .Where(t => t.Feedbacks.Any(f => f.SellerId == seller.SellerId))
                    .Select(t => new TenderDropdownViewModel
                    {
                        TenderId = t.TenderId,
                        TenderTitle = t.TenderTitle
                    })
                    .ToListAsync();

                // Calculate feedback statistics
                var totalFeedback = await _context.Feedbacks
                    .CountAsync(f => f.SellerId == seller.SellerId);

                var withComments = await _context.Feedbacks
                    .CountAsync(f => f.SellerId == seller.SellerId
                        && !string.IsNullOrEmpty(f.Comment));

                var recent = await _context.Feedbacks
                    .CountAsync(f => f.SellerId == seller.SellerId
                        && f.FeedbackDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-30)));

                // Calculate positive feedback
                var positiveWords = new[] { "good", "great", "excellent", "awesome",
                    "amazing", "perfect", "outstanding", "professional", "recommend" };

                var positiveFeedback = await _context.Feedbacks
                    .Where(f => f.SellerId == seller.SellerId && !string.IsNullOrEmpty(f.Comment))
                    .ToListAsync();

                var positiveFeedbackCount = positiveFeedback
                    .Count(f => positiveWords.Any(word =>
                        f.Comment.Contains(word, StringComparison.OrdinalIgnoreCase)));

                // Calculate average rating
                var averageRating = await _context.Feedbacks
                    .Where(f => f.SellerId == seller.SellerId && f.Rating.HasValue)
                    .AverageAsync(f => (double?)f.Rating) ?? 0;

                var feedbackStats = new FeedbackStatsViewModel
                {
                    Total = totalFeedback,
                    WithComments = withComments,
                    Recent = recent,
                    PositiveFeedback = positiveFeedbackCount,
                    AverageRating = Math.Round(averageRating, 1)
                };

                var model = new FeedbackIndexViewModel
                {
                    Feedbacks = feedbacks,
                    FeedbackStats = feedbackStats,
                    SellerTenders = sellerTenders,
                    SelectedRating = rating,
                    SelectedTender = tender
                };

                return View(model); // Looks for Views/Seller/Feedback.cshtml
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading feedback.");
                return View(new FeedbackIndexViewModel());
            }
        }

        #endregion
    }
}