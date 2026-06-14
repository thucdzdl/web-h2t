using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly string _connectionString;
        public PlaylistController(IConfiguration configuration) => _connectionString = configuration.GetConnectionString("DefaultConnection");

        // Lấy chi tiết 1 Playlist (kèm các bài hát bên trong)
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetPlaylistDetail(int id)
        {
            var songs = new List<object>();
            string playlistName = "Playlist của tôi";
            string playlistDesc = "Danh sách nhạc yêu thích";

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                
                // 1. Lấy thông tin Playlist
                var cmdName = new SqlCommand("SELECT Name, Description FROM Playlists WHERE Id = @Id", conn);
                cmdName.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmdName.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        playlistName = reader["Name"].ToString();
                        playlistDesc = reader["Description"]?.ToString();
                    }
                }

                // 2. Lấy danh sách bài hát trong Playlist đó
                var query = "SELECT VideoId, Title, Artist, Thumbnail FROM PlaylistSongs WHERE PlaylistId = @Id ORDER BY AddedAt DESC";
                var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        songs.Add(new { 
                            id = reader["VideoId"].ToString(), 
                            title = reader["Title"].ToString(), 
                            artist = reader["Artist"].ToString(), 
                            thumbnail = reader["Thumbnail"].ToString() 
                        });
                    }
                }
            }
            return Ok(new { name = playlistName, desc = playlistDesc, songs = songs });
        }
        // API: Thêm bài hát vào Playlist
        [HttpPost("add-song")]
        public async Task<IActionResult> AddSongToPlaylist([FromBody] PlaylistSongDto request)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    
                    // Kiểm tra xem bài này đã có trong Playlist chưa để tránh thêm trùng
                    var checkCmd = new SqlCommand("SELECT COUNT(1) FROM PlaylistSongs WHERE PlaylistId = @PlaylistId AND VideoId = @VideoId", conn);
                    checkCmd.Parameters.AddWithValue("@PlaylistId", request.PlaylistId);
                    checkCmd.Parameters.AddWithValue("@VideoId", request.VideoId);
                    int exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                    if (exists > 0)
                    {
                        return BadRequest(new { message = "Bài hát này đã có trong Playlist rồi!" });
                    }

                    // Nếu chưa có thì thêm mới vào
                    var cmd = new SqlCommand(@"
                        INSERT INTO PlaylistSongs (PlaylistId, VideoId, Title, Artist, Thumbnail) 
                        VALUES (@PlaylistId, @VideoId, @Title, @Artist, @Thumbnail)", conn);
                    
                    cmd.Parameters.AddWithValue("@PlaylistId", request.PlaylistId);
                    cmd.Parameters.AddWithValue("@VideoId", request.VideoId);
                    cmd.Parameters.AddWithValue("@Title", request.Title);
                    cmd.Parameters.AddWithValue("@Artist", request.Artist);
                    cmd.Parameters.AddWithValue("@Thumbnail", request.Thumbnail);

                    await cmd.ExecuteNonQueryAsync();
                }
                return Ok(new { message = "Đã thêm vào Playlist thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
            }
        }

        // Tạo một Class nhỏ để nhận dữ liệu từ Giao diện gửi lên
        public class PlaylistSongDto
        {
            public int PlaylistId { get; set; }
            public string? VideoId { get; set; }
            public string? Title { get; set; }
            public string? Artist { get; set; }
            public string? Thumbnail { get; set; }
        }
        // BỔ SUNG HÀM NÀY ĐỂ KHỚP 100% VỚI GIAO DIỆN WEB
     [HttpGet("~/api/playlists/{username}")]
        public async Task<IActionResult> GetUserPlaylistByUsername(string username)
        {
            var songs = new List<object>();

            // CHÍNH LÀ DÒNG NÀY BỊ THIẾU KHIẾN SẾP BỊ LỖI ĐỎ ĐÓ:
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                
                var query = "SELECT VideoId, Title, Artist, Thumbnail FROM PlaylistSongs ORDER BY Id DESC";
                
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            songs.Add(new { 
                                videoId = reader["VideoId"].ToString(),
                                title = reader["Title"].ToString(), 
                                artist = reader["Artist"].ToString(), 
                                thumbnail = reader["Thumbnail"].ToString() 
                            });
                        }
                    }
                }
            }
            
            return Ok(songs); 
        }
    }
}
