using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Backend.Models;
using BCrypt.Net; 

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MusicAppDbContext _context;

        public UsersController(MusicAppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    try
    {
        // 1. Kiểm tra trùng Username
        var userExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
        if (userExists)
            return BadRequest(new { message = "Tên đăng nhập đã tồn tại!" });

        // 2. BỔ SUNG: Kiểm tra trùng Email để tránh SQL ném lỗi 500
        var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (emailExists)
            return BadRequest(new { message = "Email này đã được sử dụng cho một tài khoản khác!" });

        // 3. Mã hóa mật khẩu (Hash)
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 4. Đóng gói thông tin người dùng mới (Gán rõ các cột mới)
        var newUser = new User
        {
            Username = request.Username, 
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = "User",           // Cố định người mới đăng ký là User thường
            IsPremium = false,       // Chưa có VIP
            CreatedAt = DateTime.Now // Khớp thời gian với SQL
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng ký thành công!" });
    }
    catch (Exception ex)
    {
        // Nếu có lỗi bất ngờ, in thẳng ra Console để dễ bắt bệnh chứ không sập ngầm
        Console.WriteLine($"[Lỗi Đăng Ký]: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"[Chi Tiết Lỗi]: {ex.InnerException.Message}");
        
        return StatusCode(500, new { message = "Lỗi Server, vui lòng xem Console của Backend!" });
    }
}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Tìm user theo tên đăng nhập (Username) của bạn
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Kiểm tra mật khẩu (So sánh mật khẩu trần với mật khẩu đã băm trong DB)
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác!" });
            }

            // 3. Đăng nhập thành công (Trả về dữ liệu xịn của bạn)
            return Ok(new { 
                message = "Đăng nhập thành công!", 
                // Gửi kèm trạng thái IsPremium về cho Frontend luôn
                user = new { user.Id, user.Username, user.Email, user.IsPremium } 
            });
        }

        // =========================================================
        // TÍNH NĂNG MỚI: API NÂNG CẤP TÀI KHOẢN PREMIUM
        // =========================================================
        [HttpPost("upgrade-premium")]
        public async Task<IActionResult> UpgradePremium([FromBody] UpgradeRequest request)
        {
            try
            {
                // Tìm tài khoản cần nâng cấp
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
                
                if (user == null)
                    return NotFound(new { message = "Không tìm thấy tài khoản để nâng cấp." });

                // Bật cờ Premium
                user.IsPremium = true; 
                await _context.SaveChangesAsync();

                return Ok(new { message = "Nâng cấp H2T Premium thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
            }
        }
    }

    // =========================================================
    // CÁC CLASS REQUEST TỪ FRONTEND
    // =========================================================
    public class RegisterRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    // Class mới để hứng dữ liệu tên tài khoản lúc bấm mua Premium
    public class UpgradeRequest
    {
        public string Username { get; set; } = null!;
    }
}