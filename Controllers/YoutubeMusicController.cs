using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Common;
using MusicApp.Backend.Models;
namespace MusicApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YoutubeMusicController : ControllerBase
    {
        private readonly YoutubeClient _youtube = new YoutubeClient();

        // API 1: Tìm kiếm bài hát
        [HttpGet("search")]
        public async Task<IActionResult> SearchSongs([FromQuery] string keyword)
        {
            keyword = keyword + " audio";
            var results = await _youtube.Search.GetVideosAsync(keyword).CollectAsync(10); // Lấy 10 bài
            
            var songs = results.Select(v => new 
            {
                Id = v.Id.Value,
                Title = v.Title,
                Artist = v.Author.ChannelTitle,
                Thumbnail = v.Thumbnails.FirstOrDefault()?.Url
            });

            return Ok(songs);
        }
        // API 3: Lấy danh sách nhạc Thịnh hành từ Playlist
    [HttpGet("trending")]
    public async Task<IActionResult> GetTrendingPlaylist()
    {
        try
        {
            // Mã Playlist xịn mà Thức vừa tìm được
            var playlistId = "PLRhunSKoxxzZjPu8cvOUCgd-I3pP1M8A9";
            
            // Dùng _youtube cho khớp với file của bạn. Kéo 15 bài đầu tiên.
            var videos = await _youtube.Playlists.GetVideosAsync(playlistId).CollectAsync(15);
            
            var songs = videos.Select(v => new
            {
                id = v.Id.Value,
                title = v.Title,
                artist = v.Author.ChannelTitle,
                thumbnail = v.Thumbnails.FirstOrDefault()?.Url
            });

            return Ok(songs);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


        // API 2: Lấy link nhạc Audio để web phát
       [HttpGet("play/{videoId}")]
        public async Task<IActionResult> PlayVideo(string videoId)
        {
            var youtube = new YoutubeClient();

            try
            {
                // 1. Kịch bản A: Cố gắng lấy nhạc từ ID gốc thật nhanh
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                
                return Ok(new { streamUrl = audioStreamInfo.Url });
            }
            catch (Exception ex)
            {
                // BẮT ĐẦU KỊCH BẢN B: LÁCH LUẬT BẢN QUYỀN
                Console.WriteLine($"\n[Cảnh báo] Bị YouTube chặn ID gốc ({videoId}): {ex.Message}");
                Console.WriteLine("[Hệ thống] Đang ngầm tìm bản Audio/Lyrics thay thế...");

                try
                {
                    // Lấy tên bài hát và ca sĩ của video gốc đang bị chặn
                    var video = await youtube.Videos.GetAsync(videoId);
                    
                    // Tạo từ khóa tìm kiếm mới lách bản quyền (Thêm chữ "audio")
                    string searchQuery = $"{video.Title} {video.Author} audio";
                    
                    // Nhờ YouTube tìm hộ bản khác
                    var searchResults = await youtube.Search.GetVideosAsync(searchQuery);
                    var fallbackVideo = searchResults.FirstOrDefault();

                    if (fallbackVideo != null)
                    {
                        Console.WriteLine($"[Thành công] Đã tìm thấy bản lách luật: {fallbackVideo.Title}");
                        
                        // Lấy luồng nhạc của bản thay thế này trả về cho Frontend
                        var fallbackManifest = await youtube.Videos.Streams.GetManifestAsync(fallbackVideo.Id);
                        var fallbackAudioInfo = fallbackManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                        
                        return Ok(new { streamUrl = fallbackAudioInfo.Url });
                    }
                    else
                    {
                        return StatusCode(500, "Hệ thống đã cố gắng nhưng không tìm được bản thay thế!");
                    }
                }
                catch (Exception fallbackEx)
                {
                    // Nếu xui xẻo bản thay thế cũng bị chặn nốt
                    Console.WriteLine($"[Lỗi nặng] Thất bại hoàn toàn: {fallbackEx.Message}");
                    return StatusCode(500, "Bài hát này bị đánh bản quyền quá mạnh, không thể phát!");
                }
            }
            
        }
        [HttpGet("download/{videoId}")]
public async Task<IActionResult> DownloadVideo(string videoId)
{
    try
    {
        // Đã thêm "đồ nghề" YoutubeClient vào đây để máy chạy
        var youtube = new YoutubeExplode.YoutubeClient(); 

        var video = await youtube.Videos.GetAsync(videoId);
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
        var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        if (audioStreamInfo == null) return NotFound(new { message = "Không tìm thấy dữ liệu âm thanh." });

        var stream = await youtube.Videos.Streams.GetAsync(audioStreamInfo);
        string safeTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));
        
        return File(stream, "audio/mpeg", enableRangeProcessing: true);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "Lỗi Server: " + ex.Message });
    }
}
        // =========================================================
        // VỊ TRÍ 1: DÁN API "THẢ TIM" VÀO NGAY DƯỚI API TRENDING
        // =========================================================
        [HttpPost("toggle-favorite")]
public async Task<IActionResult> ToggleFavorite([FromBody] FavoriteRequest request)
{
    // Đừng quên đổi tên Database thành tên đúng của bạn ở SSMS nhé
    string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;";

    try
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("usp_ToggleFavorite", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@VideoId", request.VideoId);
                cmd.Parameters.AddWithValue("@Title", request.Title);
                cmd.Parameters.AddWithValue("@Artist", request.Artist);
                cmd.Parameters.AddWithValue("@Thumbnail", request.Thumbnail);
                cmd.Parameters.AddWithValue("@Username", request.Username);

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();
                return Ok(new { status = result?.ToString() });
            }
        }
        
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = ex.Message });
    }
}
    // =========================================================
        // API LẤY DANH SÁCH BÀI HÁT YÊU THÍCH (Sắp xếp mới nhất lên đầu)
        // =========================================================
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoriteSongs()
        {
            // Đã điền sẵn tên Database MusicAppDB chuẩn xác cho bạn
            string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
            var favoriteSongs = new List<FavoriteSong>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Lệnh SQL lấy toàn bộ nhạc, bài nào thả tim sau cùng sẽ xếp lên trên (DESC)
                    string sql = "SELECT Id, VideoId, Title, Artist, Thumbnail, LikedAt FROM FavoriteSongs ORDER BY LikedAt DESC";
                    
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                favoriteSongs.Add(new FavoriteSong
                                {
                                    Id = reader.GetInt32(0),
                                    VideoId = reader.GetString(1),
                                    Title = reader.GetString(2),
                                    // Kiểm tra xem Artist/Thumbnail có bị trống (NULL) trong DB không
                                    Artist = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Thumbnail = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    LikedAt = reader.GetDateTime(5)
                                });
                            }
                        }
                    }
                }
                return Ok(favoriteSongs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // =========================================================
        // API ĐẾM TỔNG SỐ BÀI HÁT YÊU THÍCH
        // =========================================================
        // =========================================================
        // API ĐẾM TỔNG SỐ BÀI HÁT YÊU THÍCH (THEO USER)
        // =========================================================
        [HttpGet("favorites/count")]
        public async Task<IActionResult> GetFavoriteSongsCount([FromQuery] string username)
        {
            // Nếu Frontend không gửi tên user lên, báo lỗi luôn
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Thiếu thông tin người dùng.");
            }

            string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Lệnh T-SQL chỉ đếm nhạc của ĐÚNG người này
                    string sql = "SELECT COUNT(*) FROM FavoriteSongs WHERE Username = @Username";
                    
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username); // Truyền tham số an toàn
                        await conn.OpenAsync();
                        
                        int count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        return Ok(new { total = count });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        
        // =========================================================
        // API LẤY DANH SÁCH NGHỆ SĨ (Đã tích hợp AvatarUrl)
        // =========================================================
        [HttpGet("artists")]
        public async Task<IActionResult> GetAllArtists()
        {
            string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
            var artists = new List<object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Lấy thêm cột AvatarUrl
                    string sql = "SELECT Id, Name, AvatarUrl FROM Artists"; 
                    
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                artists.Add(new {
                                    Id = reader["Id"],
                                    Name = reader["Name"].ToString(),
                                    // Kiểm tra xem AvatarUrl có bị NULL trong SQL không
                                    AvatarUrl = reader["AvatarUrl"] != DBNull.Value ? reader["AvatarUrl"].ToString() : null
                                });
                            }
                        }
                    }
                }
                return Ok(artists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // =========================================================
        // API LẤY DANH SÁCH BÀI HÁT CỦA 1 NGHỆ SĨ
        // =========================================================
        [HttpGet("artists/{artistId}/songs")]
        public async Task<IActionResult> GetSongsByArtist(int artistId)
        {
            string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;";
            var songs = new List<object>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Lọc ra đúng những bài hát có ArtistId khớp với tham số truyền vào
                    string sql = "SELECT Id, Title, AudioUrl, Genre FROM Songs WHERE ArtistId = @ArtistId"; 
                    
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Truyền tham số an toàn để chống hack (SQL Injection)
                        cmd.Parameters.AddWithValue("@ArtistId", artistId);
                        
                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                songs.Add(new {
                                    Id = reader["Id"],
                                    Title = reader["Title"].ToString(),
                                    AudioUrl = reader["AudioUrl"].ToString(),
                                    Genre = reader["Genre"] != DBNull.Value ? reader["Genre"].ToString() : "Chưa cập nhật"
                                });
                            }
                        }
                    }
                }
                return Ok(songs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // ==========================================
        // API LẤY DANH SÁCH ALBUM
        // ==========================================
        [HttpGet("albums")]
        public IActionResult GetAllAlbums()
        {
            try
            {
                var albums = new List<object>();
                // Thay chuỗi kết nối này bằng chuỗi bạn đang dùng ở các hàm trên nhé
                string connectionString = "Server=.;Database=MusicAppDB;Trusted_Connection=True;TrustServerCertificate=True;"; 
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Id, Title, CoverUrl FROM Albums";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                albums.Add(new
                                {
                                    id = reader["Id"],
                                    title = reader["Title"].ToString(),
                                    coverUrl = reader["CoverUrl"] != DBNull.Value ? reader["CoverUrl"].ToString() : null
                                });
                            }
                        }
                    }
                }
                return Ok(albums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
    
    }
    public class FavoriteRequest
    {
        public string VideoId { get; set; }= default!;
        public string Title { get; set; }= default!;
        public string? Artist { get; set; }
        public string? Thumbnail { get; set; }
       public string? Username { get; set; }
    }
    
    