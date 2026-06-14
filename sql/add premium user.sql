-- Lệnh này thêm cột IsPremium kiểu BIT (chỉ nhận giá trị 0 hoặc 1)
-- Bạn nhớ đổi chữ 'Users' thành đúng tên bảng chứa tài khoản của bạn (ví dụ: TaiKhoan, Account...)
ALTER TABLE Users
ADD IsPremium BIT NOT NULL DEFAULT 0;
SELECT * FROM Users;