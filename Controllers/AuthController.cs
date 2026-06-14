using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using MusicApp.Backend.Models;

namespace MusicApp.Backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MusicAppDbContext _context;

        public AuthController(MusicAppDbContext context) 
        { 
            _context = context; 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // BƯỚC 1: Tìm người dùng trong Database CHỈ bằng Email (Chưa vội xét mật khẩu)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Username);

            if (user == null) 
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu!" });
            }

            // BƯỚC 2: Kiểm tra mật khẩu thông minh (Bao thầu cả acc cũ lẫn acc mới)
            bool isPasswordValid = false;

            // Nếu dữ liệu trong DB có dạng "$2a$..." thì nó là mật khẩu đã bị mã hóa (của acc mới đăng ký)
            if (user.PasswordHash.StartsWith("$2a$") || user.PasswordHash.StartsWith("$2b$") || user.PasswordHash.StartsWith("$2y$"))
            {
                // Dùng máy quét BCrypt để dịch và đối chiếu
                isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            }
            else
            {
                // Nếu không có "$2a$", tức là tài khoản test nhập bằng tay (thuc, khachhang...) -> So sánh trực tiếp
                isPasswordValid = (user.PasswordHash == request.Password);
            }

            // BƯỚC 3: Phán quyết
            if (!isPasswordValid) 
            {
                return Unauthorized(new { message = "Sai tài khoản hoặc mật khẩu!" });
            }

            // Đăng nhập thành công, gom Data gửi về cho trình duyệt
            return Ok(new { 
                id = user.Id, 
                name = user.Username, 
                email = user.Email, 
                role = user.Role 
            });
        }
    }
}