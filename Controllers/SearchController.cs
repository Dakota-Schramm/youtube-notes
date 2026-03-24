using Microsoft.AspNetCore.Mvc;

namespace youtube_notes.Controllers;

public class SearchController : Controller
{
    public IActionResult Index(string? url)
    {
        ViewData["VideoUrl"] = ConvertToEmbedUrl(url);
        return View();
    }

    private static string? ConvertToEmbedUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var videoId = query["v"];
            if (!string.IsNullOrEmpty(videoId))
                return $"https://www.youtube.com/embed/{videoId}";
        }

        return null;
    }
}
