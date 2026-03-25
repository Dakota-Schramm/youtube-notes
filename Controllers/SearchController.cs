using Microsoft.AspNetCore.Mvc;
using youtube_notes.Data;
using youtube_notes.Models;

namespace youtube_notes.Controllers;

public class SearchController : Controller
{
    private readonly youtube_notesContext _context;

    public SearchController(youtube_notesContext context)
    {
        _context = context;
    }

    public IActionResult Index(string? url)
    {
        if (!isValidUrl(url))
        {
            ViewData["Error"] = "Please enter a valid YouTube URL.";
            return View();
        }

        if (_context.Note == null)
        {
            ViewData["Error"] = "Database connection error. Please try again later.";
            return View();
        }


        ViewData["VideoUrl"] = ConvertToEmbedUrl(url);

        ViewData["Notes"] = FetchNotesforVideo(ViewData["VideoUrl"]?.ToString() ?? string.Empty);

        return View();
    }

    [HttpPost]
    public IActionResult Create(string comment, string youtubeUrl, int timeAt = 0)
    {
        if (_context.Note == null)
        {
            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
                return BadRequest(new { error = "Database connection error." });
            return RedirectToAction("Index", new { url = youtubeUrl });
        }

        var videoId = ExtractVideoId(ConvertToEmbedUrl(youtubeUrl));

        var note = new Note
        {
            Comment = comment,
            YoutubeId = videoId,
            TimeAt = timeAt
        };

        _context.Note.Add(note);
        _context.SaveChanges();

        if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            return Json(new { note.Id, note.Comment, note.TimeAt });

        return RedirectToAction("Index", new { url = youtubeUrl });
    }

    // TODO: Fix this to only handle url string form (?)
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

    // TODO: Extract text after "/" and get rid of query params from embedded url
    private static string? ExtractVideoId(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;
        
        // extract text after / and before "?" if it exists
        string videoId = url.Split('/').LastOrDefault()?.Split('?').FirstOrDefault() ?? string.Empty;

        return videoId;
    }

    private Note[] FetchNotesforVideo(string? url)
    {
        string videoId = ExtractVideoId(url);
        var notes = from n in _context.Note
                    where n.YoutubeId == videoId
                    orderby n.TimeAt ascending
                    select n;

        return notes.ToArray();

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
