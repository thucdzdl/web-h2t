using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Backend.Models;

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly MusicAppDbContext _context;

        public CommunityController(MusicAppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. API LẤY DANH SÁCH BÌNH LUẬN (Cập nhật liên tục)
        // ==========================================
        [HttpGet("comments")]
        public async Task<IActionResult> GetComments()
        {
            // Lấy 50 bình luận mới nhất, sắp xếp theo thời gian
            var comments = await _context.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Take(50)
                .ToListAsync();
            
            return Ok(comments);
        }

        // ==========================================
        // 2. API NHẬN BÌNH LUẬN MỚI TỪ NGƯỜI DÙNG
        // ==========================================
        [HttpPost("comments")]
        public async Task<IActionResult> PostComment([FromBody] CommentRequest request)
        {
            // Chống spam tin nhắn trống
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { message = "Nội dung bình luận không được để trống!" });

            // Đóng gói tin nhắn mới
            var newComment = new Comment
            {
                Username = request.Username,
                Content = request.Content,
                CreatedAt = DateTime.Now // Tự động lấy giờ hệ thống lúc đăng
            };

            // Lưu vào SQL Server
            _context.Comments.Add(newComment);
            await _context.SaveChangesAsync();

            // Trả về tin nhắn vừa tạo để frontend hiển thị luôn cho nóng
            return Ok(new { message = "Đã gửi bình luận!", comment = newComment });
        }
    }

    // ==========================================
    // CLASS HỨNG DỮ LIỆU TỪ FRONTEND GỬI LÊN
    // ==========================================
    public class CommentRequest
    {
        public string Username { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}