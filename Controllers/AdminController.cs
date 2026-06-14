using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
// Dòng quyết định sinh tử: Gọi đúng thư mục Models chứa User.cs và DbContext!
using MusicApp.Backend.Models; 

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly MusicAppDbContext _context;

        public AdminController(MusicAppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. API CHO TRANG TỔNG QUAN (DASHBOARD)
        // ==========================================
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalSongs = await _context.Songs.CountAsync(); 

            return Ok(new {
                TotalUsers = totalUsers,
                TotalSongs = totalSongs,
                TotalRevenue = 12500000, 
                TodayListeners = 8302
            });
        }

        // ==========================================
        // 2. API CHO TRANG QUẢN LÝ USER (LẤY DANH SÁCH)
        // ==========================================
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsPremium,
                    u.Role 
                })
                .ToListAsync();

            return Ok(users);
        }

        // ==========================================
        // 3. API KHÓA / MỞ KHÓA TÀI KHOẢN (ĐỒNG BỘ MỚI)
        // Đường dẫn: POST api/admin/users/toggle-lock/{id}
        // ==========================================
        [HttpPost("users/toggle-lock/{id}")]
        public async Task<IActionResult> ToggleLockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy User!" });

            // Logic lật trạng thái: Nếu đang bị Banned thì thả xích về User, ngược lại thì Ban
            if (user.Role == "Banned")
            {
                user.Role = "User"; // Trở lại trạng thái người dùng thường hoạt động
            }
            else
            {
                user.Role = "Banned"; // Chuyển sang trạng thái khóa
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái khóa thành công!", currentRole = user.Role });
        }

        // ==========================================
        // 3B. API CẤP / HỦY QUYỀN PREMIUM (BỔ SUNG MỚI)
        // Đường dẫn: POST api/admin/users/toggle-premium/{id}
        // ==========================================
        [HttpPost("users/toggle-premium/{id}")]
        public async Task<IActionResult> TogglePremiumStatus(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "Không tìm thấy User!" });

            // Logic lật trạng thái Premium: Đang true thành false, đang false thành true
            user.IsPremium = !user.IsPremium;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật trạng thái Premium thành công!", isPremium = user.IsPremium });
        }

        // ==========================================
        // 4. API CHO TRANG QUẢN LÝ BÀI HÁT
        // ==========================================
        [HttpGet("songs")]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _context.Songs.ToListAsync();
            return Ok(songs);
        }

        // ==========================================
        // 5. API XÓA BÀI HÁT
        // ==========================================
        [HttpDelete("songs/{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null) return NotFound(new { message = "Không tìm thấy bài hát!" });

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xóa bài hát thành công!" });
        }

        // ==========================================
        // 6. API CHO TRANG QUẢN LÝ GIAO DỊCH (Mock Data)
        // ==========================================
        [HttpGet("transactions")]
        public IActionResult GetAllTransactions()
        {
            var transactions = new[]
            {
                new { id = 1, code = "#H2T-89231", username = "thucdzai", packageName = "Gói Sinh Viên (1 Tháng)", amount = 29000, date = "Vừa xong", status = "Success" },
                new { id = 2, code = "#H2T-89230", username = "hieu_nguyen", packageName = "Gói Tiêu Chuẩn (1 Năm)", amount = 590000, date = "2 giờ trước", status = "Success" },
                new { id = 3, code = "#H2T-89229", username = "nam_coder", packageName = "Gói Gia Đình (1 Tháng)", amount = 109000, date = "5 giờ trước", status = "Pending" },
                new { id = 4, code = "#H2T-89228", username = "guest_7749", packageName = "Gói Tiêu Chuẩn (1 Tháng)", amount = 0, date = "Hôm qua", status = "Failed" }
            };

            return Ok(transactions);
        }

        // ==========================================
        // 7. API THÊM BÀI HÁT MỚI
        // ==========================================
        [HttpPost("songs")]
        public async Task<IActionResult> AddSong([FromBody] Song newSong)
        {
            if (string.IsNullOrWhiteSpace(newSong.Title) || string.IsNullOrWhiteSpace(newSong.AudioUrl))
            {
                return BadRequest(new { message = "Tên bài hát và Link URL không được để trống!" });
            }

            if (string.IsNullOrWhiteSpace(newSong.Genre)) newSong.Genre = "Chưa phân loại";

            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm bài hát thành công!", song = newSong });
        }
    }
}