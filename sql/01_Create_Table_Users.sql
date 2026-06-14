-- 1. Chỉ định sử dụng đúng Database của dự án
USE MusicAppDB;
GO

-- 2. Xóa các bảng phụ thuộc trước để không bị dính lỗi ràng buộc Khóa ngoại (Foreign Key)
IF OBJECT_ID('PlaylistSongs', 'U') IS NOT NULL DROP TABLE PlaylistSongs;
IF OBJECT_ID('Playlists', 'U') IS NOT NULL DROP TABLE Playlists;
IF OBJECT_ID('FavoriteSongs', 'U') IS NOT NULL DROP TABLE FavoriteSongs;
IF OBJECT_ID('ListenHistory', 'U') IS NOT NULL DROP TABLE ListenHistory;
IF OBJECT_ID('Comments', 'U') IS NOT NULL DROP TABLE Comments;

-- 3. Xóa bảng Users cũ để làm sạch nền móng
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
GO

-- 4. Tạo bảng Users chuẩn hóa hoàn chỉnh cho đồ án
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,     -- Chống trùng tên đăng nhập khi người khác đăng ký
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,       -- Chống trùng Email
    Role NVARCHAR(20) NOT NULL DEFAULT 'User', -- Mặc định đăng ký qua web sẽ là 'User'
    IsPremium BIT NOT NULL DEFAULT 0,          -- Mặc định đăng ký qua web sẽ là tài khoản thường (0)
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- 5. Chèn duy nhất tài khoản cá nhân của bạn mang cả 2 vương miện quyền lực nhất
INSERT INTO Users (Username, PasswordHash, Email, Role, IsPremium)
VALUES (N'thuc', N'123456', N'thuc@gmail.com', N'Admin', 1);
GO

-- 6. Kiểm tra lại danh sách tài khoản trong kho dữ liệu
SELECT * FROM Users;