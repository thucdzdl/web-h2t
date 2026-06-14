using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Backend.Models;

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly MusicAppDbContext _context;

        public FavoritesController(MusicAppDbContext context)
        {
            _context = context;
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteRequest request)
        {
            try
            {
                var existingFavorite = await _context.FavoriteSongs
                    .FirstOrDefaultAsync(f => f.Username == request.Username && f.VideoId == request.VideoId);

                if (existingFavorite != null)
                {
                    _context.FavoriteSongs.Remove(existingFavorite);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Đã bỏ thích bài hát", isFavorited = false });
                }
                
                var newFavorite = new FavoriteSong
                {
                    Username = request.Username,
                    VideoId = request.VideoId,
                    Title = request.Title,
                    Artist = request.Artist,
                    Thumbnail = request.Thumbnail,
                    LikedAt = DateTime.Now
                };
                
                _context.FavoriteSongs.Add(newFavorite);
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "Đã thêm vào danh sách yêu thích", isFavorited = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetFavorites(string username)
        {
            var favorites = await _context.FavoriteSongs
                .Where(f => f.Username == username)
                .OrderByDescending(f => f.LikedAt)
                .ToListAsync();

            return Ok(favorites);
        }
    }

    public class ToggleFavoriteRequest
    {
        public string Username { get; set; } = null!;
        public string VideoId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Artist { get; set; }
        public string? Thumbnail { get; set; }
    }
}