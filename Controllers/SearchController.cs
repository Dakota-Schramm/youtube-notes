using Microsoft.AspNetCore.Mvc;
using youtube_notes.Models;

namespace youtube_notes.Controllers;

public class SearchController : Controller
{
    public IActionResult Index(string? url)
    {
        if (!isValidUrl(url))
        {
            ViewData["Error"] = "Please enter a valid YouTube URL.";
            return View();
        }

        ViewData["VideoUrl"] = ConvertToEmbedUrl(url);
        ViewData["Notes"] = FetchNotesforVideo(ViewData["VideoUrl"]?.ToString() ?? string.Empty);

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

    private static Note[] FetchNotesforVideo(string videoId)
    {
        // This is a placeholder. In a real application, you would query your database for notes associated with the videoId.
        return new Note[]
        {
            new Note { Id = 1, Comment = "This is a great video!", YoutubeId = videoId, TimeAt = 30 },
            new Note { Id = 2, Comment = "I learned a lot from this.", YoutubeId = videoId, TimeAt = 120 }
        };
    }

    private static bool isValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return false;

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            if (uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be"))
                return true;
        }

        return false;
    }
}
