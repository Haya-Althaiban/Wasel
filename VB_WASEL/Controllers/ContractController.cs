using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.ContractVMs;

namespace Wasel.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class ContractController : BaseController
    {
        public ContractController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Contracts

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

                // Get contracts through bids
                var contracts = await _context.Contracts
                    .Include(c => c.Bid)
                        .ThenInclude(b => b.Tender)
                            .ThenInclude(t => t.Buyer)
                    .Include(c => c.Bid)
                        .ThenInclude(b => b.Tender)
                    .Where(c => c.Bid.SellerId == seller.SellerId)
                    .OrderByDescending(c => c.StartDate)
                    .Select(c => new ContractListItemViewModel
                    {
                        ContractId = c.ContractId,
                        BidId = c.BidId,
                        TenderTitle = c.Bid.Tender.TenderTitle,
                        BuyerName = c.Bid.Tender.Buyer.BuyerName,
                        ContractValue = c.ContractValue ?? 0,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate,
                        Status = c.Status,
                        PaymentTerms = c.PaymentTerms,
                        DeliverySchedule = c.DeliverySchedule
                    })
                    .ToListAsync();

                var model = new ContractIndexViewModel
                {
                    Contracts = contracts
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading contracts.");
                return View(new ContractIndexViewModel());
            }
        }

        #endregion

        #region Review Contract

        [HttpGet]
        public async Task<IActionResult> Review(int id)
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

                var contract = await _context.Contracts
                    .Include(c => c.Bid)
                        .ThenInclude(b => b.Tender)
                            .ThenInclude(t => t.Buyer)
                    .Include(c => c.Payments)
                    .Include(c => c.Bid)
                        .ThenInclude(b => b.Tender)
                            .ThenInclude(t => t.Criteria)
                    .FirstOrDefaultAsync(c => c.ContractId == id);

                if (contract == null)
                {
                    SetErrorMessage("Contract not found.");
                    return RedirectToAction("Index");
                }

                // Verify contract belongs to seller
                if (contract.Bid.SellerId != seller.SellerId)
                {
                    SetErrorMessage("Unauthorized action.");
                    return RedirectToAction("Index");
                }

                // Calculate contract statistics
                var totalPayments = contract.Payments.Sum(p => p.Amount ?? 0);
                var remainingAmount = (contract.ContractValue ?? 0) - totalPayments;
                var completionPercentage = contract.ContractValue > 0
                    ? (double)totalPayments / (double)contract.ContractValue.Value * 100
                    : 0;

                var model = new ContractReviewViewModel
                {
                    ContractId = contract.ContractId,
                    BidId = contract.BidId,
                    TenderTitle = contract.Bid.Tender.TenderTitle,
                    TenderDescription = contract.Bid.Tender.TenderDescription,
                    BuyerName = contract.Bid.Tender.Buyer.BuyerName,
                    BuyerEmail = contract.Bid.Tender.Buyer.User.Email,
                    BuyerPhone = contract.Bid.Tender.Buyer.ContactPhone,
                    ContractValue = contract.ContractValue ?? 0,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    Status = contract.Status,
                    PaymentTerms = contract.PaymentTerms,
                    DeliverySchedule = contract.DeliverySchedule,
                    ContractDocumentUrl = contract.ContractDocumentUrl,
                    Criteria = contract.Bid.Tender.Criteria.Select(c => new CriteriaViewModel
                    {
                        CriteriaName = c.CriteriaName,
                        CriteriaDescription = c.CriteriaDescription,
                        Weight = c.Weight ?? 0,
                        DeliveryTime = c.DeliveryTime
                    }).ToList(),
                    Payments = contract.Payments.Select(p => new PaymentSummaryViewModel
                    {
                        PaymentNum = p.PaymentNum,
                        PaymentDate = p.PaymentDate,
                        Amount = p.Amount ?? 0,
                        PaymentStatus = p.PaymentStatus
                    }).ToList(),
                    TotalPayments = totalPayments,
                    RemainingAmount = remainingAmount,
                    CompletionPercentage = Math.Round(completionPercentage, 2)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading contract details.");
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Update Contract Status

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateContractStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Invalid status data.");
                return RedirectToAction("Review", new { id = model.ContractId });
            }

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

                var contract = await _context.Contracts
                    .Include(c => c.Bid)
                    .FirstOrDefaultAsync(c => c.ContractId == model.ContractId);

                if (contract == null)
                {
                    SetErrorMessage("Contract not found.");
                    return RedirectToAction("Index");
                }

                // Verify contract belongs to seller
                if (contract.Bid.SellerId != seller.SellerId)
                {
                    SetErrorMessage("Unauthorized action.");
                    return RedirectToAction("Index");
                }

                contract.Status = model.Status;
                _context.Contracts.Update(contract);
                await _context.SaveChangesAsync();

                var statusMessage = model.Status switch
                {
                    "Approved" => "Contract approved successfully!",
                    "Rejected" => "Contract rejected.",
                    "Pending" => "Contract status updated to pending.",
                    _ => "Status updated."
                };

                SetSuccessMessage(statusMessage);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to update contract status.");
                return RedirectToAction("Review", new { id = model.ContractId });
            }
        }

        #endregion
    }
}