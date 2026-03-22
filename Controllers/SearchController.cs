using Microsoft.AspNetCore.Mvc;

namespace youtube_notes.Controllers;

public class SearchController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
