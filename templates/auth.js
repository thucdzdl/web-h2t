document.addEventListener("DOMContentLoaded", function() {
    const authSection = document.getElementById("auth-section");
    if (!authSection) return;

    // Lấy thông tin user từ Trí nhớ trình duyệt (LocalStorage)
    const userJson = localStorage.getItem("h2t_user");

    if (userJson) {
        // TRƯỜNG HỢP 1: ĐÃ ĐĂNG NHẬP
        console.log("Dữ liệu từ LocalStorage:", userJson);
        const user = JSON.parse(userJson);

       // Kiểm tra xem User này có phải Admin không? 
        let adminBtnHtml = '';
        if (user.role && user.role.toLowerCase() === 'admin') {
            adminBtnHtml = `
                <a href="admin_dashboard.html" title="Vào trang Quản Trị" style="color: inherit; text-decoration: none; margin-right: 15px;">
                    <i class="fas fa-user-shield" style="color: #1db954; font-size: 20px; cursor: pointer;"></i>
                </a>
            `;
        }

        // Vẽ giao diện: Khiên Admin (nếu có) + Tên User + Nút Đăng xuất
        authSection.innerHTML = `
            <div style="display: flex; align-items: center;">
                ${adminBtnHtml}
                <span style="color: white; font-weight: bold; margin-right: 15px;">Xin chào, ${user.name}!</span>
                <button id="btn-logout" style="background: transparent; border: 1px solid #b3b3b3; color: white; padding: 5px 15px; border-radius: 20px; cursor: pointer; transition: 0.3s;">Đăng xuất</button>
            </div>
        `;

        // Gắn chức năng cho nút Đăng xuất
        document.getElementById("btn-logout").addEventListener("click", function() {
            localStorage.removeItem("h2t_user"); // Xóa trí nhớ
            window.location.reload(); // F5 lại trang
        });

    } else {
        // TRƯỜNG HỢP 2: CHƯA ĐĂNG NHẬP
        // Chỉ hiện nút Đăng nhập và Đăng ký, không có tên tuổi hay khiên Admin gì hết
        authSection.innerHTML = `
            <a href="login.html" class="btn-login" style="background: white; color: black; padding: 10px 25px; border-radius: 25px; font-weight: bold; text-decoration: none; display: inline-block;">Đăng nhập</a>
            <a href="register.html" class="btn-register" style="color: white; margin-left: 15px; text-decoration: none; font-weight: bold;">Đăng ký</a>
        `;
    }
});
// ==========================================
        // HỆ THỐNG FREEMIUM: GIỚI HẠN 3 LƯỢT NGHE
        // ==========================================
        function checkFreemiumLimit() {
            // 1. KIỂM TRA RADAR: Xem khách đã đăng nhập chưa
            // Quét LocalStorage tìm cục dữ liệu chứa email và role (do auth.js tạo ra)
            let isLoggedIn = false;
            for (let i = 0; i < localStorage.length; i++) {
                let val = localStorage.getItem(localStorage.key(i));
                if (val && val.includes('"email"') && val.includes('"role"')) {
                    isLoggedIn = true; 
                    break;
                }
            }

            // Nếu là VIP (đã đăng nhập) -> Mở barie cho qua ngay lập tức
            if (isLoggedIn) {
                return true; 
            }

            // 2. KHÁCH VÃNG LAI: Kiểm tra số vé đã dùng
            let playCount = localStorage.getItem('guestPlayCount');
            playCount = playCount ? parseInt(playCount) : 0;

            if (playCount >= 3) {
                // 3. THU LƯỚI: Đã dùng hết 3 lượt -> Khóa mỏ!
                alert("🛑 TRẢI NGHIỆM ĐÃ KẾT THÚC!\n\nBạn đã sử dụng hết 3 lượt nghe thử miễn phí. Vui lòng đăng nhập hoặc tạo tài khoản H2T Premium để nghe nhạc không giới hạn nhé!");
                window.location.href = "login.html"; // Đá văng sang trang đăng nhập
                return false; // Lệnh này cực kỳ quan trọng để ngắt luồng không cho nhạc kêu
            }

            // 4. CẤP VÉ MỚI: Nếu chưa đủ 3 bài, cộng thêm 1 lượt và cho qua
            localStorage.setItem('guestPlayCount', playCount + 1);
            console.log("Khách vãng lai đang nghe bài thứ: " + (playCount + 1) + "/3");
            return true;
        }