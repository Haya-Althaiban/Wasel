using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Wasel.Controllers;
using Wasel.Data;
using Wasel.Services;
using Wasel.ViewModels.SellerVMs.MessageVMs;

namespace Wasel.Areas.Seller.Controllers
{
    #region MessageController

    [Area("Seller")]
    [Authorize(Policy = "SellerOnly")]
    public class MessageController : BaseController
    {
        public MessageController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Index - List Messages

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = CurrentUserId.Value;

                var messages = await _context.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.SentDate)
                    .Select(m => new SellerMessageItemViewModel
                    {
                        MessageNum = m.MessageNum,
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        SenderName = m.Sender.UserName,
                        ReceiverName = m.Receiver.UserName,
                        MessageText = m.MessageText,
                        SentDate = m.SentDate ?? DateTime.Now,
                        IsRead = m.IsRead,
                        IsSentByMe = m.SenderId == userId
                    })
                    .ToListAsync();

                var model = new SellerMessageIndexViewModel
                {
                    Messages = messages
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred while loading messages.");
                return View(new SellerMessageIndexViewModel());
            }
        }

        #endregion

        #region Store Message

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Store(SendSellerMessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage("Please fill in all required fields.");
                return RedirectToAction("Index");
            }

            try
            {
                var message = new Models.Message
                {
                    UserId = CurrentUserId.Value,
                    SenderId = CurrentUserId.Value,
                    ReceiverId = model.ReceiverId,
                    MessageText = model.MessageText,
                    SentDate = DateTime.Now,
                    IsRead = false
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                SetSuccessMessage("Message sent successfully.");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("Failed to send message. Please try again.");
                return RedirectToAction("Index");
            }
        }

        #endregion
    }

    #endregion
}