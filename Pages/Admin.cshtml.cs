using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace WebLoginLab.Pages;

[Authorize(Roles = "Admin")]
public class AdminModel : PageModel
{
	public void OnGet()
	{
	}
}
