using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebLoginLab.Pages;

public class LoginModel : PageModel
{
	private static readonly Dictionary<string, (string password, string role)>
		Accounts = new(StringComparer.Ordinal)
		{
			["Admin"] = ("admin114514", "Admin"),
			["USER1"] = ("user1919810", "User")
		};


	[BindProperty]
	[Required(ErrorMessage = "请输入用户名")]
	public string Username { get; set; } = "";

	[BindProperty]
	[Required(ErrorMessage ="请输入密码")]
	public string Password { get; set; } = "";

	[BindProperty(SupportsGet = true)]
	public string? ReturnUrl { get; set; }


	//处理Post请求 async表异步
	public async Task<IActionResult> OnPostAsync()
	{
        //必要数据先验 检查上面标记必要的BindProperty的数据是否存在，以及有无Reqiured不合规等
		//后续允许继续向ModelState中添加错误以返回，但需要额外的检查逻辑。
        if (!ModelState.IsValid)
		{
			return Page();
		}

		//根据传入的用户名预账号密码进行审查判断
		if(!Accounts.TryGetValue(Username, out var acc) || !acc.password.Equals(Password))
		{
			ModelState.AddModelError("","账号或密码错误");
			return Page();
		}

		//信息构造
		var claims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, Username),
			new Claim(ClaimTypes.Name, Username),
			new Claim(ClaimTypes.Role, acc.role)
		};

		//信息实体
		var idt = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		//角色实体 能存多个信息实体
		var principal = new ClaimsPrincipal(idt);

		//签发
		await HttpContext.SignInAsync(
			CookieAuthenticationDefaults.AuthenticationScheme,
			principal,
			new AuthenticationProperties { IsPersistent = false }
		);

		//界面重定向回退
		var redirct = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : "/private";

		return LocalRedirect(redirct);
	}

	public LoginModel()
	{
	}
}
