using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Wasel.Data;
using Wasel.Models;
using Wasel.Services;
using Wasel.ViewModels.AuthVMs;

namespace Wasel.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(
            WaselDbContext context,
            INotificationService notificationService,
            IErrorLoggingService errorLoggingService)
            : base(context, notificationService, errorLoggingService)
        {
        }

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectBasedOnUserType(CurrentUser);
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user == null || !VerifyPassword(model.Password, user.Password))
                {
                    ModelState.AddModelError("", "The provided credentials do not match our records.");
                    return View(model);
                }

                await SignInUserAsync(user, model.RememberMe);

                HttpContext.Session.SetString("LastActivityTime", DateTime.Now.ToString());

                SetSuccessMessage($"Welcome back, {user.UserName}!");

                return RedirectBasedOnUserType(user);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetErrorMessage("An error occurred during login. Please try again.");
                return View(model);
            }
        }

        #endregion

        #region Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(model);
                }

                // Create User
                var user = new User
                {
                    UserName = model.Name,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    UserType = model.UserType
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Create Buyer profile
                if (model.UserType == "buyer" || model.UserType == "both")
                {
                    var buyer = new Models.Buyer
                    {
                        UserId = user.UserId,
                        BuyerName = model.BuyerName ?? model.Name,
                        ContactPhone = model.ContactPhone ?? model.Phone,
                        BuyerCity = model.BuyerCity,
                        BuyerAddress = model.BuyerAddress
                    };
                    _context.Buyers.Add(buyer);
                }

                // Create Seller profile
                if (model.UserType == "seller" || model.UserType == "both")
                {
                    var seller = new Seller
                    {
                        UserId = user.UserId,
                        SellerName = model.SellerName ?? model.Name,
                        ContactPhone = model.SellerPhone ?? model.Phone,
                        SellerCity = model.SellerCity,
                        SellerAddress = model.SellerAddress
                    };
                    _context.Sellers.Add(seller);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Auto login
                await SignInUserAsync(user, false);

                if (model.UserType == "both")
                {
                    SetSuccessMessage("Account created successfully! Please choose your dashboard.");
                    return RedirectToAction("DashboardSelector");
                }

                SetSuccessMessage("Account created successfully!");
                return RedirectBasedOnUserType(user);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                LogError(ex);
                SetErrorMessage($"Registration failed: {ex.Message}");
                return View(model);
            }
        }

        #endregion

        #region Profile Management

        [HttpGet]
        [Authorize]
        public IActionResult Profile()
        {
            var user = CurrentUser;
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var buyer = _context.Buyers.FirstOrDefault(b => b.UserId == user.UserId);
            var seller = _context.Sellers.FirstOrDefault(s => s.UserId == user.UserId);

            var model = new ProfileViewModel
            {
                Name = user.UserName,
                Email = user.Email,
                UserType = user.UserType,
                BuyerName = buyer?.BuyerName,
                ContactPhone = buyer?.ContactPhone,
                BuyerCity = buyer?.BuyerCity,
                BuyerAddress = buyer?.BuyerAddress,
                SellerName = seller?.SellerName,
                SellerPhone = seller?.ContactPhone,
                SellerCity = seller?.SellerCity,
                SellerAddress = seller?.SellerAddress
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = CurrentUser;
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if email is taken by another user
                if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.UserId != user.UserId))
                {
                    ModelState.AddModelError("Email", "This email is already taken.");
                    return View(model);
                }

                // Update User
                user.UserName = model.Name;
                user.Email = model.Email;
                _context.Users.Update(user);

                // Update Buyer profile
                if (user.UserType == "buyer" || user.UserType == "both")
                {
                    var buyer = await _context.Buyers.FirstOrDefaultAsync(b => b.UserId == user.UserId);
                    if (buyer != null)
                    {
                        buyer.BuyerName = model.BuyerName;
                        buyer.ContactPhone = model.ContactPhone;
                        buyer.BuyerCity = model.BuyerCity;
                        buyer.BuyerAddress = model.BuyerAddress;
                        _context.Buyers.Update(buyer);
                    }
                }

                // Update Seller profile
                if (user.UserType == "seller" || user.UserType == "both")
                {
                    var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserId == user.UserId);
                    if (seller != null)
                    {
                        seller.SellerName = model.SellerName;
                        seller.ContactPhone = model.SellerPhone;
                        seller.SellerCity = model.SellerCity;
                        seller.SellerAddress = model.SellerAddress;
                        _context.Sellers.Update(seller);
                    }
                }

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
                return View(model);
            }
        }

        #endregion

        #region Dashboard Selector (for "both" users)

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DashboardSelector()
        {
            var user = CurrentUser;
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.UserType != "both")
            {
                return RedirectBasedOnUserType(user);
            }

            var buyer = await _context.Buyers
                .Include(b => b.Tenders)
                .FirstOrDefaultAsync(b => b.UserId == user.UserId);

            var seller = await _context.Sellers
                .Include(s => s.Bids)
                .FirstOrDefaultAsync(s => s.UserId == user.UserId);

            var model = new DashboardSelectorViewModel
            {
                UserName = user.UserName,
                BuyerTendersCount = buyer?.Tenders?.Count ?? 0,
                SellerBidsCount = seller?.Bids?.Count ?? 0
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult SwitchDashboard(string dashboardType)
        {
            var user = CurrentUser;
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (user.UserType != "both")
            {
                return RedirectBasedOnUserType(user);
            }

            if (dashboardType != "buyer" && dashboardType != "seller")
            {
                SetErrorMessage("Invalid dashboard type.");
                return RedirectToAction("DashboardSelector");
            }

            HttpContext.Session.SetString("current_dashboard", dashboardType);

            SetSuccessMessage($"Switched to {dashboardType} dashboard");

            return dashboardType switch
            {
                "buyer" => RedirectToAction("Index", "Dashboard", new { area = "Buyer" }),
                "seller" => RedirectToAction("Index", "Dashboard", new { area = "Seller" }),
                _ => RedirectToAction("DashboardSelector")
            };
        }

        #endregion

        #region Logout

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            SetInfoMessage("You have been logged out successfully.");
            return RedirectToAction("Login");
        }

        #endregion

        #region Helper Methods

        private async Task SignInUserAsync(User user, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserType", user.UserType)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = isPersistent,
                ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private IActionResult RedirectBasedOnUserType(User user)
        {
            return user.UserType.ToLower() switch
            {
                "admin" => RedirectToAction("Index", "Support"),
                "buyer" => RedirectToAction("Index", "Dashboard", new { area = "Buyer" }),
                "seller" => RedirectToAction("Index", "Dashboard", new { area = "Seller" }),
                "both" => RedirectToAction("DashboardSelector"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput.Equals(hashedPassword);
        }

        #endregion
    }
}