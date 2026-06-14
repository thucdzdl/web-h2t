using System;

namespace MusicApp.Backend.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!; // Tên người bình luận
        public string Content { get; set; } = null!;  // Nội dung bình luận
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian đăng
    }
}