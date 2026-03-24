using System.ComponentModel.DataAnnotations;

namespace youtube_notes.Models;

public class Note
{
    public int Id { get; set; }
    public string Comment { get; set; }

    [Display(Name="YouTube Video ID")]
    public string YoutubeId { get; set; }

    [Display(Name="Time At (seconds)")]
    public int TimeAt { get; set; }
}
