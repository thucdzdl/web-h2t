-- 1. Trỏ đúng vào Database của bạn
USE MusicAppDB;
GO

-- 2. Tạo bảng lưu trữ bài hát yêu thích
CREATE TABLE FavoriteSongs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VideoId VARCHAR(50) NOT NULL UNIQUE,
    Title NVARCHAR(250) NOT NULL,
    Artist NVARCHAR(250),
    Thumbnail VARCHAR(500),
    LikedAt DATETIME DEFAULT GETDATE()
);
GO

-- 3. Tạo Stored Procedure xử lý Thả tim / Bỏ tim
CREATE PROCEDURE usp_ToggleFavorite
    @VideoId VARCHAR(50),
    @Title NVARCHAR(250),
    @Artist NVARCHAR(250),
    @Thumbnail VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM FavoriteSongs WHERE VideoId = @VideoId)
    BEGIN
        DELETE FROM FavoriteSongs WHERE VideoId = @VideoId;
        SELECT 'Unliked' AS Result;
    END
    ELSE
    BEGIN
        INSERT INTO FavoriteSongs (VideoId, Title, Artist, Thumbnail)
        VALUES (@VideoId, @Title, @Artist, @Thumbnail);
        SELECT 'Liked' AS Result;
    END
END
GO