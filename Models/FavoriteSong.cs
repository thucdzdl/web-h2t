using System;
using System.ComponentModel.DataAnnotations;

namespace MusicApp.Backend.Models
{
    public class FavoriteSong
    {
        [Key]
        public int Id { get; set; }
        
        public string Username { get; set; } = string.Empty;
        
        public string VideoId { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string? Artist { get; set; }
        
        public string? Thumbnail { get; set; }
        
        public DateTime LikedAt { get; set; } = DateTime.Now;
    }
}