using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // 框架认证方案，当前选项为Cookies方案
    .AddCookie(opt => // Cookies 方案注册
    {
        // 配置默认的权限审查路径：未登录与权限不足
        opt.LoginPath = "/login";
        opt.AccessDeniedPath = "/";

        opt.Cookie.Name = "WebLoginLab.Auth"; // Cookie保存路径
        opt.Cookie.HttpOnly = true;
        opt.Cookie.SecurePolicy = CookieSecurePolicy.Always; // 要求https进行请求发送
        opt.Cookie.SameSite = SameSiteMode.Lax;

        opt.ExpireTimeSpan = TimeSpan.FromMinutes(30); //三十分钟登录有效性
        opt.SlidingExpiration = true; //滑动：成功验证时允许服务器适时签发新cookie刷新有效期
    });

builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
