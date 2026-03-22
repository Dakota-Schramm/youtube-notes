using Microsoft.AspNetCore.Mvc;

namespace youtube_notes.Controllers;

public class SearchController : Controller
{
    public IActionResult Index(string? url)
    {
        ViewData["VideoUrl"] = url;
        return View();
    }
}
