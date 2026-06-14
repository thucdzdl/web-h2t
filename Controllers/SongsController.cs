using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Backend.Models;
using Microsoft.AspNetCore.Http; 
using Microsoft.AspNetCore.Hosting; 
using System.IO;

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly MusicAppDbContext _context;
        private readonly IWebHostEnvironment _env; // Biến môi trường

        // Cập nhật hàm khởi tạo để nhận IWebHostEnvironment
        public SongsController(MusicAppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSongs()
        {
            var songs = await _context.Songs.ToListAsync();
            return Ok(songs);
        }

        [HttpPost]
        public async Task<IActionResult> AddSong([FromBody] Song newSong)
        {
            _context.Songs.Add(newSong);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm bài hát thành công!", song = newSong });
        }

        // --- TÍNH NĂNG MỚI: API UPLOAD FILE ---
        // Method: POST - URL: http://localhost:5043/api/songs/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Vui lòng chọn một file để upload!");

            // 1. Chỉ định nơi lưu trữ: Thư mục "wwwroot/music"
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "music");
            
            // Nếu thư mục chưa tồn tại, tự động tạo mới
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Tạo tên file mới để tránh bị trùng lặp (ví dụ: bị ghi đè khi 2 user cùng up bài "abc.mp3")
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // 3. Tiến hành copy file từ Ram vào Ổ cứng
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // 4. Trả về đường dẫn tĩnh (để Frontend dùng gắn vào Database)
            var fileUrl = $"/music/{uniqueFileName}";
            return Ok(new { message = "Upload file thành công!", fileUrl = fileUrl });
        }
    }
}