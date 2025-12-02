using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wasel.Data;
using Wasel.Models;
using Wasel.Services;
using Wasel.ViewModels.BuyerVMs;

namespace Wasel.Controllers.Buyer
{
    [Authorize]
    public class BuyerController : BaseController
    {
        public BuyerController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        /// <summary>
        /// Display buyer dashboard
        /// </summary>
        [HttpGet]
        public IActionResult Dashboard()
        {
            try
            {
                if (!HasRole("buyer"))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                var buyer = _context.Buyers
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.UserId == CurrentUserId);

                if (buyer == null)
                {
                    SetErrorMessage("ملف المشتري غير موجود");
                    return RedirectToAction("Login", "Account");
                }

                // Get dashboard statistics
                var stats = new BuyerDashboardViewModel
                {
                    Buyer = buyer,
                    ActiveTenders = _context.Tenders
                        .Count(t => t.BuyerId == buyer.BuyerId && t.TenderStatus == "Open"),

                    TotalBidsReceived = _context.Bids
                        .Count(b => b.Tender.BuyerId == buyer.BuyerId),

                    TotalTenders = _context.Tenders
                        .Count(t => t.BuyerId == buyer.BuyerId),

                    ClosedTenders = _context.Tenders
                        .Count(t => t.BuyerId == buyer.BuyerId && t.TenderStatus == "Closed"),

                    RecentTenders = _context.Tenders
                        .Where(t => t.BuyerId == buyer.BuyerId)
                        .OrderByDescending(t => t.CreatedDate)
                        .Take(5)
                        .Select(t => new RecentTenderViewModel
                        {
                            TenderId = t.TenderId,
                            TenderTitle = t.TenderTitle,
                            TenderStatus = t.TenderStatus,
                            PublishDate = t.PublishDate,
                            SubmissionDeadline = t.SubmissionDeadline,
                            TenderBudget = t.TenderBudget,
                            BidsCount = t.Bids.Count
                        })
                        .ToList(),

                    ChartData = GetChartData(buyer.BuyerId)
                };

                return View(stats);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("حدث خطأ أثناء تحميل لوحة التحكم");
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Get real chart data from database
        /// </summary>
        private ChartDataViewModel GetChartData(int buyerId)
        {
            var currentDate = DateTime.Now;
            var chartData = new ChartDataViewModel();

            for (int i = 11; i >= 0; i--)
            {
                var date = currentDate.AddMonths(-i);
                var monthName = date.ToString("MMM");

                chartData.Months.Add(monthName);

                // Tenders created in this month
                var tendersCount = _context.Tenders
                    .Count(t => t.BuyerId == buyerId &&
                                t.CreatedDate.HasValue &&
                                t.CreatedDate.Value.Year == date.Year &&
                                t.CreatedDate.Value.Month == date.Month);
                chartData.Tenders.Add(tendersCount);

                // Bids received in this month
                var bidsCount = _context.Bids
                    .Count(b => b.Tender.BuyerId == buyerId &&
                                b.SubmissionDate.HasValue &&
                                b.SubmissionDate.Value.Year == date.Year &&
                                b.SubmissionDate.Value.Month == date.Month);
                chartData.Bids.Add(bidsCount);

                // Contracts created in this month
                var contractsCount = _context.Contracts
                    .Count(c => c.Bid.Tender.BuyerId == buyerId &&
                                c.StartDate.HasValue &&
                                c.StartDate.Value.Year == date.Year &&
                                c.StartDate.Value.Month == date.Month);
                chartData.Contracts.Add(contractsCount);
            }

            return chartData;
        }

        /// <summary>
        /// Display buyer profile
        /// </summary>
        [HttpGet]
        public IActionResult Profile()
        {
            try
            {
                if (!HasRole("buyer"))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                var buyer = _context.Buyers
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.UserId == CurrentUserId);

                if (buyer == null)
                {
                    SetErrorMessage("ملف المشتري غير موجود");
                    return RedirectToAction("Dashboard");
                }

                var viewModel = new BuyerProfileViewModel
                {
                    BuyerId = buyer.BuyerId,
                    Name = buyer.BuyerName,
                    Email = buyer.User?.Email,
                    Phone = buyer.ContactPhone,
                    City = buyer.BuyerCity,
                    Address = buyer.BuyerAddress
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("حدث خطأ أثناء تحميل الملف الشخصي");
                return RedirectToAction("Dashboard");
            }
        }

        /// <summary>
        /// Update buyer profile
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(BuyerProfileViewModel model)
        {
            try
            {
                if (!HasRole("buyer"))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View("Profile", model);
                }

                var buyer = _context.Buyers
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.UserId == CurrentUserId);

                if (buyer == null)
                {
                    SetErrorMessage("ملف المشتري غير موجود");
                    return RedirectToAction("Dashboard");
                }

                // Check if email already exists for another user
                var emailExists = _context.Users
                    .Any(u => u.Email == model.Email && u.UserId != CurrentUserId);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "البريد الإلكتروني مستخدم بالفعل");
                    return View("Profile", model);
                }

                // Update User
                buyer.User.UserName = model.Name;
                buyer.User.Email = model.Email;

                // Update Buyer profile
                buyer.BuyerName = model.Name;
                buyer.ContactPhone = model.Phone;
                buyer.BuyerCity = model.City;
                buyer.BuyerAddress = model.Address;

                _context.SaveChanges();

                SetSuccessMessage("تم تحديث الملف الشخصي بنجاح!");
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("فشل تحديث الملف الشخصي: " + ex.Message);
                return View("Profile", model);
            }
        }

        /// <summary>
        /// Delete buyer account
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAccount()
        {
            try
            {
                if (!HasRole("buyer"))
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                var buyer = _context.Buyers
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.UserId == CurrentUserId);

                if (buyer == null)
                {
                    SetErrorMessage("ملف المشتري غير موجود");
                    return RedirectToAction("Dashboard");
                }

                // Delete buyer profile
                _context.Buyers.Remove(buyer);

                // Delete user account
                _context.Users.Remove(buyer.User);

                _context.SaveChanges();

                // Logout
                return RedirectToAction("Logout", "Account");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("فشل حذف الحساب: " + ex.Message);
                return RedirectToAction("Profile");
            }
        }
    }
}