using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace WebLoginLab.Pages;

[Authorize]
public class PrivateModel : PageModel
{
	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostLogoutAsync()
	{
		await HttpContext.SignOutAsync(
			CookieAuthenticationDefaults.AuthenticationScheme
		);

		return RedirectToPage("/Index");
	}
}
