using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebLoginLab.Pages;

public class IndexModel : PageModel
{
    public string ServerMessage { get; private set; } = "";

    public void OnGet()
    {
        ServerMessage = $"Backend data. Time: {DateTime.Now:HH:mm:ss}";
    }
}
