using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.PaymentVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class PaymentController : BaseController
    {
        public PaymentController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Payments

        [HttpGet]
        public async Task<IActionResult> Index()
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

                // Get payments through contracts and bids
                var payments = await _context.Payments
                    .Include(p => p.Contract)
                        .ThenInclude(c => c.Bid)
                            .ThenInclude(b => b.Tender)
                                .ThenInclude(t => t.Buyer)
                    .Include(p => p.Invoices)
                    .Where(p => p.Contract.Bid.SellerId == seller.SellerId)
                    .OrderByDescending(p => p.PaymentDate)
                    .Select(p => new PaymentListItemViewModel
                    {
                        PaymentNum = p.PaymentNum,
                        ContractId = p.ContractId,
                        TenderTitle = p.Contract.Bid.Tender.TenderTitle,
                        BuyerName = p.Contract.Bid.Tender.Buyer.BuyerName,
                        PaymentDate = p.PaymentDate,
                        Amount = p.Amount ?? 0,
                        BuyerCommission = p.BuyerCommission ?? 0,
                        SellerCommission = p.SellerCommission ?? 0,
                        NetAmount = p.NetAmount ?? 0,
                        PaymentStatus = p.PaymentStatus,
                        HasInvoice = p.Invoices.Any(),
                        InvoiceNum = p.Invoices.FirstOrDefault() != null
                            ? p.Invoices.FirstOrDefault().InvoiceNum
                            : (int?)null
                    })
                    .ToListAsync();

                // Calculate payment statistics
                var totalPayments = payments.Sum(p => p.NetAmount);
                var pendingPayments = payments.Count(p => p.PaymentStatus == "Pending");
                var completedPayments = payments.Count(p => p.PaymentStatus == "Completed");
                var totalCommission = payments.Sum(p => p.SellerCommission);

                var paymentStats = new PaymentStatsViewModel
                {
                    TotalPayments = totalPayments,
                    PendingPayments = pendingPayments,
                    CompletedPayments = completedPayments,
                    TotalCommission = totalCommission,
                    PaymentCount = payments.Count
                };

                var model = new PaymentIndexViewModel
                {
                    Payments = payments,
                    PaymentStats = paymentStats
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading payments.");
                return View(new PaymentIndexViewModel());
            }
        }

        #endregion

        #region Invoice

        [HttpGet]
        public async Task<IActionResult> Invoice(int id)
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

                var payment = await _context.Payments
                    .Include(p => p.Contract)
                        .ThenInclude(c => c.Bid)
                            .ThenInclude(b => b.Tender)
                                .ThenInclude(t => t.Buyer)
                                    .ThenInclude(b => b.User)
                    .Include(p => p.Contract)
                        .ThenInclude(c => c.Bid)
                            .ThenInclude(b => b.Seller)
                    .Include(p => p.Invoices)
                    .FirstOrDefaultAsync(p => p.PaymentNum == id);

                if (payment == null)
                {
                    SetErrorMessage("Payment not found.");
                    return RedirectToAction("Index");
                }

                // Verify payment belongs to seller
                if (payment.Contract.Bid.SellerId != seller.SellerId)
                {
                    SetErrorMessage("Unauthorized action.");
                    return RedirectToAction("Index");
                }

                var invoice = payment.Invoices.FirstOrDefault();

                var model = new PaymentInvoiceViewModel
                {
                    PaymentNum = payment.PaymentNum,
                    InvoiceNum = invoice?.InvoiceNum ?? 0,
                    InvoiceDate = invoice?.InvoiceDate,
                    InvoiceTime = invoice?.InvoiceTime,

                    // Contract details
                    ContractId = payment.ContractId,
                    ContractValue = payment.Contract.ContractValue ?? 0,

                    // Tender details
                    TenderTitle = payment.Contract.Bid.Tender.TenderTitle,
                    TenderDescription = payment.Contract.Bid.Tender.TenderDescription,

                    // Buyer details
                    BuyerName = payment.Contract.Bid.Tender.Buyer.BuyerName,
                    BuyerAddress = payment.Contract.Bid.Tender.Buyer.BuyerAddress,
                    BuyerCity = payment.Contract.Bid.Tender.Buyer.BuyerCity,
                    BuyerPhone = payment.Contract.Bid.Tender.Buyer.ContactPhone,
                    BuyerEmail = payment.Contract.Bid.Tender.Buyer.User.Email,

                    // Seller details
                    SellerName = payment.Contract.Bid.Seller.SellerName,
                    SellerAddress = payment.Contract.Bid.Seller.SellerAddress,
                    SellerCity = payment.Contract.Bid.Seller.SellerCity,
                    SellerPhone = payment.Contract.Bid.Seller.ContactPhone,

                    // Payment details
                    PaymentDate = payment.PaymentDate,
                    Amount = payment.Amount ?? 0,
                    BuyerCommission = payment.BuyerCommission ?? 0,
                    SellerCommission = payment.SellerCommission ?? 0,
                    NetAmount = payment.NetAmount ?? 0,
                    PaymentStatus = payment.PaymentStatus
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading invoice.");
                return RedirectToAction("Index");
            }
        }

        #endregion
    }
}