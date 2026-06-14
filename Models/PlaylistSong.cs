using System;
using System.Collections.Generic;

namespace MusicApp.Backend.Models;

public partial class PlaylistSong
{
    public int PlaylistId { get; set; }

    public int SongId { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Song Song { get; set; } = null!;
}
