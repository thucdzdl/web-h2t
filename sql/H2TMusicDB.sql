-- 1. Tạo Database
create database MusicAppDB;

-- 2. Chỉ định sử dụng Database vừa tạo
use MusicAppDB;

-- 3. Tạo bảng Người dùng
create table Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) DEFAULT 'User'
);

-- 4. Tạo bảng Nghệ sĩ
CREATE TABLE Artists (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Bio NVARCHAR(MAX),
    AvatarUrl NVARCHAR(500)
);

-- 5. Tạo bảng Album
CREATE TABLE Albums (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    ReleaseDate DATE,
    ArtistId INT FOREIGN KEY REFERENCES Artists(Id),
    CoverImageUrl NVARCHAR(500)
);

-- 6. Tạo bảng Bài hát
CREATE TABLE Songs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Duration INT, -- Lưu tổng thời lượng bài hát (tính bằng giây)
    AudioUrl NVARCHAR(500) NOT NULL,
    AlbumId INT FOREIGN KEY REFERENCES Albums(Id),
    Genre NVARCHAR(50)
);

-- 7. Tạo bảng Playlist cá nhân
CREATE TABLE Playlists (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    UserId INT FOREIGN KEY REFERENCES Users(Id),
    IsPublic BIT DEFAULT 0 -- 0: Riêng tư, 1: Công khai
);

-- 8. Tạo bảng trung gian (Một Playlist có nhiều Bài hát và ngược lại)
CREATE TABLE PlaylistSongs (
    PlaylistId INT FOREIGN KEY REFERENCES Playlists(Id),
    SongId INT FOREIGN KEY REFERENCES Songs(Id),
    AddedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (PlaylistId, SongId)
);
USE MusicAppDB; -- Thay bằng tên Database của bạn
GO

-- Xóa bài hát "Chạy ngay đi"
DELETE FROM Songs WHERE Title = N'Chạy ngay đi';

-- Nếu muốn xóa hết để làm lại từ đầu cho sạch (cẩn thận khi dùng):
-- DELETE FROM Songs;