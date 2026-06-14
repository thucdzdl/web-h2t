using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MusicApp.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly string _connectionString;

        public HistoryController(IConfiguration configuration)
        {
            // Lấy chuỗi kết nối từ file appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // 1. API LƯU LỊCH SỬ: POST api/history
        [HttpPost]
        public async Task<IActionResult> SaveHistory([FromBody] HistoryDto model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.VideoId))
                return BadRequest("Dữ liệu không hợp lệ.");

            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                // Kiểm tra xem bài hát này đã từng được user nghe chưa, nếu có thì xóa cũ để đẩy lên đầu
                var deleteQuery = "DELETE FROM Histories WHERE Username = @Username AND VideoId = @VideoId";
                using (var deleteCmd = new SqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@Username", model.Username);
                    deleteCmd.Parameters.AddWithValue("@VideoId", model.VideoId);
                    await deleteCmd.ExecuteNonQueryAsync();
                }

                // Chèn lượt nghe mới vào bảng
                var insertQuery = @"INSERT INTO Histories (Username, VideoId, Title, Artist, Thumbnail, ListenedAt) 
                    VALUES (@Username, @VideoId, @Title, @Artist, @Thumbnail, GETDATE())";
                using (var insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@Username", model.Username);
                    insertCmd.Parameters.AddWithValue("@VideoId", model.VideoId);
                    insertCmd.Parameters.AddWithValue("@Title", model.Title);
                    insertCmd.Parameters.AddWithValue("@Artist", model.Artist);
                    insertCmd.Parameters.AddWithValue("@Thumbnail", model.Thumbnail ?? (object)DBNull.Value);

                    await insertCmd.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { message = "Đã lưu vào lịch sử." });
        }

        // 2. API LẤY LỊCH SỬ: GET api/history/{username}
        [HttpGet("{username}")]
        public async Task<IActionResult> GetHistory(string username)
        {
            var list = new List<object>();

            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"SELECT TOP 20 VideoId, Title, Artist, Thumbnail 
                              FROM Histories
                              WHERE Username = @Username 
                              ORDER BY ListenedAt DESC"; // Bài mới nghe lên đầu

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new
                            {
                                id = reader["VideoId"].ToString(),
                                title = reader["Title"].ToString(),
                                artist = reader["Artist"].ToString(),
                                thumbnail = reader["Thumbnail"].ToString()
                            });
                        }
                    }
                }
            }

            return Ok(list);
        }
    }

    // Đối tượng nhận dữ liệu từ Frontend gửi lên
    public class HistoryDto
    {
        public string? Username { get; set; }
        public string? VideoId { get; set; }
        public string? Title { get; set; }
        public string? Artist { get; set; }
        public string? Thumbnail { get; set; }
    }
}