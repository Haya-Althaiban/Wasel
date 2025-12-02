using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.DashboardVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class SellerController : BaseController
    {
        public SellerController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Dashboard

        [HttpGet]
        public async Task<IActionResult> Dashboard()
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

                // Get dashboard statistics
                var stats = new DashboardStatsViewModel
                {
                    ActiveBids = await _context.Bids
                        .CountAsync(b => b.SellerId == seller.SellerId && !b.Contracts.Any()),

                    TotalBids = await _context.Bids
                        .CountAsync(b => b.SellerId == seller.SellerId),

                    AwardedContracts = await _context.Contracts
                        .Include(c => c.Bid)
                        .CountAsync(c => c.Bid.SellerId == seller.SellerId),

                    TotalRevenue = await _context.Payments
                        .Include(p => p.Contract)
                            .ThenInclude(c => c.Bid)
                        .Where(p => p.Contract.Bid.SellerId == seller.SellerId)
                        .SumAsync(p => p.NetAmount ?? 0)
                };

                // Get recent bids
                var recentBids = await _context.Bids
                    .Include(b => b.Tender)
                        .ThenInclude(t => t.Buyer)
                    .Where(b => b.SellerId == seller.SellerId)
                    .OrderByDescending(b => b.SubmissionDate)
                    .Take(5)
                    .Select(b => new RecentBidViewModel
                    {
                        BidId = b.BidId,
                        TenderTitle = b.Tender.TenderTitle,
                        BuyerName = b.Tender.Buyer.BuyerName,
                        ProposedPrice = b.ProposedPrice ?? 0,
                        SubmissionDate = b.SubmissionDate,
                        Status = b.IsApproved ? "Approved" : b.IsRejected ? "Rejected" : "Pending"
                    })
                    .ToListAsync();

                // Get open tenders count
                var openTendersCount = await _context.Tenders
                    .CountAsync(t => t.TenderStatus == "Open"
                        && t.SubmissionDeadline >= DateOnly.FromDateTime(DateTime.Now));

                // Get chart data
                var chartData = await GetChartDataAsync(seller.SellerId);

                var model = new DashboardViewModel
                {
                    Stats = stats,
                    RecentBids = recentBids,
                    OpenTendersCount = openTendersCount,
                    ChartData = chartData,
                    SellerName = seller.SellerName
                };

                return View(model); // Looks for Views/Seller/Dashboard.cshtml
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading dashboard.");
                return View(new DashboardViewModel());
            }
        }

        #endregion

        #region Profile

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = CurrentUser;
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (seller == null)
                {
                    SetErrorMessage("Seller profile not found.");
                    return RedirectToAction("Dashboard");
                }

                var model = new SellerProfileViewModel
                {
                    Name = user.UserName,
                    Email = user.Email,
                    Phone = seller.ContactPhone,
                    City = seller.SellerCity,
                    Address = seller.SellerAddress
                };

                return View(model); // Looks for Views/Seller/Profile.cshtml
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading profile.");
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(SellerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
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
                    return RedirectToAction("Dashboard");
                }

                // Check if email is taken by another user
                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != user.UserId))
                {
                    ModelState.AddModelError("Email", "This email is already taken.");
                    return View("Profile", model);
                }

                // Update User
                user.UserName = model.Name;
                user.Email = model.Email;
                _context.Users.Update(user);

                // Update Seller profile
                seller.SellerName = model.Name;
                seller.ContactPhone = model.Phone;
                seller.SellerCity = model.City;
                seller.SellerAddress = model.Address;
                _context.Sellers.Update(seller);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                SetSuccessMessage("Profile updated successfully!");
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError(ex);
                SetErrorMessage($"Profile update failed: {ex.Message}");
                return View("Profile", model);
            }
        }

        #endregion

        #region Delete Account

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccount()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = CurrentUser;
                var seller = await _context.Sellers
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (seller == null)
                {
                    SetErrorMessage("Seller profile not found.");
                    return RedirectToAction("Dashboard");
                }

                // Delete seller profile
                _context.Sellers.Remove(seller);

                // Delete user account
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Logout
                await HttpContext.SignOutAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();

                SetSuccessMessage("Your account has been deleted successfully.");
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError(ex);
                SetErrorMessage($"Account deletion failed: {ex.Message}");
                return RedirectToAction("Profile");
            }
        }

        #endregion

        #region Helper Methods

        private async Task<ChartDataViewModel> GetChartDataAsync(int sellerId)
        {
            var currentDate = DateTime.Now;
            var months = new List<string>();
            var bidsData = new List<int>();
            var contractsData = new List<int>();
            var revenueData = new List<decimal>();

            for (int i = 11; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var monthName = date.ToString("MMM");
                months.Add(monthName);

                // Bids submitted in this month
                var bidsCount = await _context.Bids
                    .Where(b => b.SellerId == sellerId
                        && b.SubmissionDate.HasValue
                        && b.SubmissionDate.Value.Year == date.Year
                        && b.SubmissionDate.Value.Month == date.Month)
                    .CountAsync();
                bidsData.Add(bidsCount);

                // Contracts awarded in this month
                var contractsCount = await _context.Contracts
                    .Include(c => c.Bid)
                    .Where(c => c.Bid.SellerId == sellerId
                        && c.StartDate.HasValue
                        && c.StartDate.Value.Year == date.Year
                        && c.StartDate.Value.Month == date.Month)
                    .CountAsync();
                contractsData.Add(contractsCount);

                // Revenue in this month
                var revenue = await _context.Payments
                    .Include(p => p.Contract)
                        .ThenInclude(c => c.Bid)
                    .Where(p => p.Contract.Bid.SellerId == sellerId
                        && p.PaymentDate.HasValue
                        && p.PaymentDate.Value.Year == date.Year
                        && p.PaymentDate.Value.Month == date.Month)
                    .SumAsync(p => p.NetAmount ?? 0);
                revenueData.Add(revenue / 1000); // Convert to thousands
            }

            return new ChartDataViewModel
            {
                Bids = bidsData,
                Contracts = contractsData,
                Revenue = revenueData,
                Months = months
            };
        }

        #endregion
    }
}