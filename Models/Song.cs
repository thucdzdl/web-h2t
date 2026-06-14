using System;
using System.Collections.Generic;

namespace MusicApp.Backend.Models;

public partial class Song
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? Duration { get; set; }

    public string AudioUrl { get; set; } = null!;

    public int? AlbumId { get; set; }

    public string? Genre { get; set; }

    public virtual Album? Album { get; set; }

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}
