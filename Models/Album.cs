using System;
using System.Collections.Generic;

namespace MusicApp.Backend.Models;

public partial class Album
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public int? ArtistId { get; set; }

    public string? CoverImageUrl { get; set; }

    public virtual Artist? Artist { get; set; }

    public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
}
