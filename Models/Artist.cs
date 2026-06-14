using System;
using System.Collections.Generic;

namespace MusicApp.Backend.Models;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();
}
