using System.ComponentModel.DataAnnotations;

namespace youtube_notes.Models;

public class Note
{
    public int Id { get; set; }
    public string Comment { get; set; }
    public string YoutubeId { get; set; }
    public int TimeAt { get; set; }
}
