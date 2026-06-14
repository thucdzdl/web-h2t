USE MusicAppDB;
GO

-- Xóa bài hát "Chạy ngay đi" (Id = 1)
DELETE FROM Songs WHERE Id = 1;

-- Xóa các bài bị trùng (Id = 3 và Id = 4)
DELETE FROM Songs WHERE Id IN (3, 4);

-- Kiểm tra lại kết quả
SELECT * FROM Songs;