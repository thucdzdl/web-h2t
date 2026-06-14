USE MusicAppDB;
GO

-- 1. Tạo lại bảng Playlists (Nối với Users qua UserId)
CREATE TABLE Playlists (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    IsPublic BIT DEFAULT 0
);
GO

-- 2. Tạo lại bảng PlaylistSongs (Nối Playlist và Bài hát nội bộ)
CREATE TABLE PlaylistSongs (
    PlaylistId INT FOREIGN KEY REFERENCES Playlists(Id),
    SongId INT FOREIGN KEY REFERENCES Songs(Id),
    AddedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (PlaylistId, SongId)
);
GO

-- 3. Tạo lại bảng Comments (Bình luận bài hát)
CREATE TABLE Comments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    VideoId VARCHAR(50) NOT NULL, -- Dùng ID của YouTube
    Content NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 4. Tạo lại bảng FavoriteSongs (Đã có cột Username chống thả tim lung tung)
CREATE TABLE FavoriteSongs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    VideoId VARCHAR(50) NOT NULL,
    Title NVARCHAR(250) NOT NULL,
    Artist NVARCHAR(250),
    Thumbnail VARCHAR(500),
    LikedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT UC_User_Video UNIQUE (Username, VideoId) -- Chống 1 người thả tim 1 bài nhiều lần
);
GO

-- 5. Tạo lại bảng ListenHistory (Lịch sử nghe nhạc)
CREATE TABLE ListenHistory (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    VideoId VARCHAR(50) NOT NULL,
    Title NVARCHAR(250) NOT NULL,
    Artist NVARCHAR(250),
    Thumbnail VARCHAR(500),
    ListenedAt DATETIME DEFAULT GETDATE()
);
GO