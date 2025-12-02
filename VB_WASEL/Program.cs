using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Wasel.Data;
using Wasel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

#region Database Configuration
builder.Services.AddDbContext<WaselDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("WaselConnection"));
});
#endregion

#region Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("UserType", "admin"));
    options.AddPolicy("BuyerOnly", policy => policy.RequireClaim("UserType", "buyer"));
    options.AddPolicy("SellerOnly", policy => policy.RequireClaim("UserType", "seller"));
});
#endregion

#region Session Configuration
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
#endregion

#region Service Registration
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IErrorLoggingService, ErrorLoggingService>();
#endregion

#region HttpContext Accessor
builder.Services.AddHttpContextAccessor();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session must come before authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

#region Custom Session Timeout Middleware
// Custom session timeout middleware (120 minutes)
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        var lastActivityTime = context.Session.GetString("LastActivityTime");

        if (!string.IsNullOrEmpty(lastActivityTime))
        {
            var lastActivity = DateTime.Parse(lastActivityTime);
            var timeSinceLastActivity = DateTime.Now - lastActivity;

            if (timeSinceLastActivity.TotalMinutes > 120) // 120 minutes timeout
            {
                // Session expired - clear session and redirect to login
                context.Session.Clear();
                context.Response.Redirect("/Auth/Login?sessionExpired=true");
                return;
            }
        }

        // Update last activity time
        context.Session.SetString("LastActivityTime", DateTime.Now.ToString());
    }

    await next();
});
#endregion

// Area routing
app.MapAreaControllerRoute(
    name: "SellerArea",
    areaName: "Seller",
    pattern: "Seller/{controller}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "BuyerArea",
    areaName: "Buyer",
    pattern: "Buyer/{controller}/{action=Index}/{id?}");

// Default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();
