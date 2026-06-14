using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicApp.Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        // Đã đổi Name thành Username cho khớp với Controller
        public string Username { get; set; } = string.Empty; 
        
        // Đã đổi Password thành PasswordHash
        public string PasswordHash { get; set; } = string.Empty; 
        
        public string Email { get; set; } = string.Empty;
        
        public string? Role { get; set; } = "User";
        
        public bool IsPremium { get; set; } = false;

        // BỔ SUNG: Thuộc tính thời gian tạo tài khoản cho khớp với SQL
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}