-- 1. Tạo bảng lưu trữ bài hát yêu thích
CREATE TABLE FavoriteSongs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VideoId VARCHAR(50) NOT NULL UNIQUE, -- ID video từ YouTube để không bị trùng lặp
    Title NVARCHAR(250) NOT NULL,
    Artist NVARCHAR(250),
    Thumbnail VARCHAR(500),
    LikedAt DATETIME DEFAULT GETDATE()
);
GO

-- 2. Tạo Stored Procedure xử lý Thêm/Xóa thông minh
CREATE PROCEDURE usp_ToggleFavorite
    @VideoId VARCHAR(50),
    @Title NVARCHAR(250),
    @Artist NVARCHAR(250),
    @Thumbnail VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    -- Nếu bài hát đã tồn tại trong bảng công việc -> Xóa (Bỏ thích)
    IF EXISTS (SELECT 1 FROM FavoriteSongs WHERE VideoId = @VideoId)
    BEGIN
        DELETE FROM FavoriteSongs WHERE VideoId = @VideoId;
        SELECT 'Unliked' AS Result;
    END
    -- Nếu chưa tồn tại -> Thêm mới (Thả tim)
    ELSE
    BEGIN
        INSERT INTO FavoriteSongs (VideoId, Title, Artist, Thumbnail)
        VALUES (@VideoId, @Title, @Artist, @Thumbnail);
        SELECT 'Liked' AS Result;
    END
END
GO