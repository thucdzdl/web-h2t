using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicApp.Backend.Models
{
public class User
{
    [Key]
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty; 
    public string PasswordHash { get; set; } = string.Empty; 
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; } = "User";
    public bool IsPremium { get; set; } = false;
    
    // ✅ THÊM DÒNG NÀY
    public bool IsLocked { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
}
}