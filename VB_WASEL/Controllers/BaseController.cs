using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Claims;
using Wasel.Data;
using Wasel.Models;
using Wasel.Services;

namespace Wasel.Controllers
{
    public class BaseController : Controller
    {
        protected readonly WaselDbContext _context;
        protected readonly INotificationService _notificationService;
        protected readonly IErrorLoggingService _errorLoggingService;

        public BaseController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
        {
            _context = context;
            _notificationService = notificationService;
            _errorLoggingService = errorLoggingService;
        }

        // Get current user ID
        protected int? CurrentUserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out int userId) ? userId : null;
            }
        }

        // Get current user
        protected User CurrentUser
        {
            get
            {
                if (CurrentUserId.HasValue)
                {
                    return _context.Users.Find(CurrentUserId.Value);
                }
                return null;
            }
        }

        // Check if user has specific role
        protected bool HasRole(string roleName)
        {
            var user = CurrentUser;
            if (user == null) return false;

            if (user.UserId == 1 && roleName != "normal")
                return true;

            return user.UserType?.Equals(roleName, StringComparison.OrdinalIgnoreCase) == true;
        }

        // Override OnActionExecuting to provide data to all views
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = CurrentUser;
                if (user != null)
                {
                    ViewBag.UnreadMessagesCount = _notificationService.GetUnreadMessagesCount(user.UserId);
                    ViewBag.Notifications = _notificationService.GetUserNotifications(user);
                    ViewBag.NotificationsCount = ViewBag.Notifications.Count;
                    ViewBag.CurrentUser = user;
                }
            }
            else
            {
                ViewBag.UnreadMessagesCount = 0;
                ViewBag.Notifications = new System.Collections.Generic.List<NotificationViewModel>();
                ViewBag.NotificationsCount = 0;
            }

            base.OnActionExecuting(context);
        }

        // Log errors using the ErrorLoggingService
        protected void LogError(Exception exception)
        {
            _errorLoggingService.LogError(exception);
        }

        // Success message
        protected void SetSuccessMessage(string message)
        {
            TempData["SuccessMessage"] = message;
        }

        // Error message
        protected void SetErrorMessage(string message)
        {
            TempData["ErrorMessage"] = message;
        }

        // Info message
        protected void SetInfoMessage(string message)
        {
            TempData["InfoMessage"] = message;
        }

        // Warning message
        protected void SetWarningMessage(string message)
        {
            TempData["WarningMessage"] = message;
        }
    }
}