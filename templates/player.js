console.log("🚀 Động cơ H2T Music Player (Global) đã khởi động!");
// Hàm xử lý Tải xuống gọi API C#
function showDownloadAlert() {
   

    // 3. Tiến hành tải nhạc
    // Tìm lấy VideoId của bài hát đang phát (thường ẩn trong thuộc tính onclick của bài hát)
    // Cách an toàn: Tìm theo ảnh bìa hiện tại đang hiển thị
    const currentImgUrl = document.getElementById('current-img').src;
    const allSongs = Array.from(document.querySelectorAll('[onclick*="getStreamAndPlay"]'));
    const currentSongElement = allSongs.find(el => el.querySelector('img').src === currentImgUrl);

    if (currentSongElement) {
        // Trích xuất VideoId từ chuỗi onclick="getStreamAndPlay('VIDEO_ID', ...)"
        const onclickText = currentSongElement.getAttribute('onclick');
        const match = onclickText.match(/getStreamAndPlay\('([^']+)'/);
        
        if (match && match[1]) {
            const videoId = match[1];
            alert("✅ Đang tiến hành tải bài hát xuống máy tính...");
            
            // GỌI API BACKEND: Ép trình duyệt mở link tải file mp3
            // Chú ý: Đảm bảo cổng 5043 đúng với cổng Backend đang chạy của bạn
            window.location.href = `http://thucpham111-001-site1.stempurl.com/api/youtubemusic/download/${videoId}`;
        } else {
            alert("Lỗi: Không tìm thấy mã bài hát để tải.");
        }
    } else {
        alert("Vui lòng chọn một bài hát để tải xuống!");
    }
}
// ==========================================
// 1. CÁC HÀM BẬT/TẮT SIDEBAR BÊN PHẢI
// ==========================================
function toggleQueue() {
    const queuePanel = document.getElementById('queue-panel');
    const lyricsPanel = document.getElementById('lyrics-panel');
    if(queuePanel) queuePanel.style.display = queuePanel.style.display === 'none' ? 'block' : 'none';
    if(lyricsPanel && queuePanel.style.display === 'block') lyricsPanel.style.display = 'none';
}

function toggleLyrics() {
    const lyricsPanel = document.getElementById('lyrics-panel');
    const queuePanel = document.getElementById('queue-panel');
    if(lyricsPanel) lyricsPanel.style.display = lyricsPanel.style.display === 'none' ? 'block' : 'none';
    if(queuePanel && lyricsPanel.style.display === 'block') queuePanel.style.display = 'none';
}
// ==========================================
// 1B. LOGIC CHUYỂN BÀI (NEXT / PREVIOUS) TỰ ĐỘNG
// ==========================================
function playNextSong() {
    // Tự động quét tất cả các bài hát đang hiển thị trên màn hình
    const allSongs = Array.from(document.querySelectorAll('[onclick*="getStreamAndPlay"]'));
    if (allSongs.length === 0) return;

    // Lấy ảnh của bài hát đang phát hiện tại dưới thanh Player
    const currentImg = document.getElementById('current-img')?.src;
    
    // Tìm vị trí (index) của bài hát đang phát trong danh sách
    let currentIndex = allSongs.findIndex(el => el.querySelector('img')?.src === currentImg);

    if (currentIndex !== -1 && currentIndex < allSongs.length - 1) {
        // Nếu tìm thấy và chưa phải bài cuối, kích hoạt click phát bài tiếp theo
        allSongs[currentIndex + 1].click();
    } else {
        // Nếu là bài cuối cùng, tự động quay vòng về bài đầu tiên
        allSongs[0].click();
    }
}

function playPrevSong() {
    const allSongs = Array.from(document.querySelectorAll('[onclick*="getStreamAndPlay"]'));
    if (allSongs.length === 0) return;

    const currentImg = document.getElementById('current-img')?.src;
    let currentIndex = allSongs.findIndex(el => el.querySelector('img')?.src === currentImg);

    if (currentIndex > 0) {
        // Lùi lại bài hát phía trước
        allSongs[currentIndex - 1].click();
    } else {
        // Nếu đang ở bài đầu tiên, bấm lùi sẽ nhảy xuống bài cuối cùng
        allSongs[allSongs.length - 1].click();
    }
}

function formatTime(seconds) {
    if (isNaN(seconds)) return "0:00";
    let min = Math.floor(seconds / 60);
    let sec = Math.floor(seconds % 60);
    if (sec < 10) sec = `0${sec}`;
    return `${min}:${sec}`;
}

// ==========================================
// 2. TẢI NHẠC THỊNH HÀNH (CHỈ CHẠY NẾU Ở TRANG THỊNH HÀNH)
// ==========================================
async function loadTrendingSongs() {
    const container = document.getElementById('trending-list');
    if (!container) return; // Nếu không phải trang Thịnh hành -> Thoát ngay để không lỗi JS

    container.innerHTML = '<tr><td colspan="5" style="color: white; text-align: center; padding: 20px;">Đang tải Bảng Xếp Hạng V-Pop...</td></tr>';
    try {
        const response = await fetch(`http://thucpham111-001-site1.stempurl.com/api/youtubemusic/trending`);
        if (!response.ok) throw new Error(`Backend lỗi: ${response.status}`);
        const songs = await response.json();
        renderTrendingList(songs);
    } catch (error) {
        container.innerHTML = `<tr><td colspan="5" style="color: red; padding: 20px;">Lỗi kết nối Backend: ${error.message}</td></tr>`;
    }
}

function renderTrendingList(songs) {
    const container = document.getElementById('trending-list');
    if(!container) return;
    container.innerHTML = ''; 
    songs.forEach((song, index) => {
        const safeTitle = song.title.replace(/'/g, ""); 
        const safeArtist = song.artist.replace(/'/g, "");
        const html = `
            <tr class="song-row" onclick="getStreamAndPlay('${song.id}', '${safeTitle}', '${safeArtist}', '${song.thumbnail}')">
                <td style="text-align: center;">${index + 1}</td>
                <td>
                    <div style="display: flex; align-items: center; gap: 15px;">
                        <img src="${song.thumbnail}" style="width: 40px; height: 40px; border-radius: 5px; object-fit: cover;">
                        <span style="color: white; font-weight: 500;">${song.title}</span>
                    </div>
                </td>
                <td style="color: #b3b3b3;">${song.artist}</td>
                <td><i class="far fa-heart hover-heart" onclick="toggleFavoriteSong(event, '${song.id}', '${safeTitle}', '${safeArtist}', '${song.thumbnail}')"></i></td>
                <td style="color: #b3b3b3;">--:--</td>
            </tr>
        `;
        container.insertAdjacentHTML('beforeend', html);
    });
}

// ==========================================
// 3. PHÁT NHẠC KHI CLICK (FULL TÍNH NĂNG)
// ==========================================
async function getStreamAndPlay(videoId, title, artist, thumbnail) {
    if (typeof checkFreemiumLimit === 'function' && !checkFreemiumLimit()) return;
    // 1. CẬP NHẬT UI THANH PLAYER Ở DƯỚI
    if(document.getElementById('current-name')) document.getElementById('current-name').innerText = title;
    if(document.getElementById('current-artist')) document.getElementById('current-artist').innerText = artist;
    if(document.getElementById('current-img')) document.getElementById('current-img').src = thumbnail;

    // ==========================================
    // 2. CẬP NHẬT BẢNG LỜI BÀI HÁT (TỰ ĐỘNG TÌM TRÊN MẠNG)
    // ==========================================
    const lyricBox = document.getElementById('lyric-content');
    if (lyricBox) {
        // A. Đặt giao diện "Đang tải" ngay lập tức để người dùng không bị chờ
        lyricBox.innerHTML = `
            <div style="text-align: center; margin-top: 20px; position: sticky; top: 0; background: #0b0b0b; z-index: 10; padding-bottom: 10px;">
                <h3 style="color: #1db954; margin-bottom: 5px; font-size: 18px;">${title}</h3>
                <p style="color: #b3b3b3; font-size: 14px; margin-top: 0;">${artist}</p>
            </div>
            <div style="margin-top: 60px; color: #fff; text-align: center;">
                <i class="fas fa-spinner fa-spin" style="font-size: 28px; color: #1db954; margin-bottom: 15px;"></i><br>
                Đang tự động tìm lời bài hát...
            </div>
        `;

        // B. Lọc tên bài hát (Xóa các chữ như (Official Video), [Audio]... để API dễ tìm hơn)
        const cleanTitle = title.replace(/\(.*?\)|\[.*?\]/g, '').trim();
        const cleanArtist = artist.replace(/\(.*?\)|\[.*?\]/g, '').trim();

        // C. Chạy ngầm gọi API miễn phí LRCLIB (Không làm chậm tốc độ load nhạc)
        fetch(`https://lrclib.net/api/get?track_name=${encodeURIComponent(cleanTitle)}&artist_name=${encodeURIComponent(cleanArtist)}`)
            .then(res => res.json())
            .then(data => {
                if (data && data.plainLyrics) {
                    // Chuyển đổi các dấu xuống dòng của API thành thẻ <br> của HTML
                    const formattedLyrics = data.plainLyrics.replace(/\n/g, '<br>');
                    lyricBox.innerHTML = `
                        <div style="text-align: center; margin-top: 20px; position: sticky; top: 0; background: #0b0b0b; z-index: 10; padding-bottom: 10px;">
                            <h3 style="color: #1db954; margin-bottom: 5px; font-size: 18px;">${title}</h3>
                            <p style="color: #b3b3b3; font-size: 14px; margin-top: 0;">${artist}</p>
                        </div>
                        <div style="margin-top: 20px; line-height: 2.2; color: #fff; font-size: 15px; font-weight: 500; text-align: center; padding-bottom: 50px;">
                            ${formattedLyrics}
                        </div>
                    `;
                } else {
                    throw new Error("API không có lời bài này");
                }
            })
            .catch(err => {
                // Nếu tìm không thấy hoặc lỗi mạng
                lyricBox.innerHTML = `
                    <div style="text-align: center; margin-top: 20px; position: sticky; top: 0; background: #0b0b0b; z-index: 10; padding-bottom: 10px;">
                        <h3 style="color: #1db954; margin-bottom: 5px; font-size: 18px;">${title}</h3>
                        <p style="color: #b3b3b3; font-size: 14px; margin-top: 0;">${artist}</p>
                    </div>
                    <div style="margin-top: 60px; color: #888; text-align: center; line-height: 1.8;">
                        <i class="fas fa-search-minus" style="font-size: 24px; margin-bottom: 15px;"></i><br>
                        Rất tiếc, hệ thống toàn cầu chưa cập nhật<br>lời cho bài hát này.
                    </div>
                `;
            });
    }

    // 3. CẬP NHẬT BẢNG HÀNG CHỜ TỰ ĐỘNG (QUEUE)
    const queueBox = document.getElementById('queue-list');
    if (queueBox) {
        let queueHTML = `
            <div style="display: flex; align-items: center; gap: 15px; background: #282828; padding: 10px; border-radius: 8px; margin-bottom: 20px;">
                <img src="${thumbnail}" style="width: 45px; height: 45px; border-radius: 5px; object-fit: cover;">
                <div style="flex: 1; overflow: hidden;">
                    <div style="color: #1db954; font-size: 11px; font-weight: bold; margin-bottom: 3px; text-transform: uppercase;">
                        <i class="fas fa-volume-up"></i> Đang phát
                    </div>
                    <div style="color: white; font-weight: bold; font-size: 14px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${title}</div>
                    <div style="color: #b3b3b3; font-size: 13px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${artist}</div>
                </div>
            </div>
            <div style="color: #b3b3b3; font-size: 12px; font-weight: bold; margin-bottom: 15px; text-transform: uppercase; letter-spacing: 1px;">
                Tiếp theo từ gợi ý
            </div>
        `;

        const allSongElements = document.querySelectorAll('[onclick*="getStreamAndPlay"]');
        let count = 0;

        allSongElements.forEach(element => {
            if (count >= 15) return;
            const onclickAttr = element.getAttribute('onclick');
            if (onclickAttr) { 
                const match = onclickAttr.match(/getStreamAndPlay\('([^']*)',\s*'([^']*)',\s*'([^']*)',\s*'([^']*)'\)/);
                if (match) {
                    const nextVideoId = match[1];
                    const nextTitle = match[2];
                    const nextArtist = match[3];
                    const nextThumb = match[4];

                    if (nextVideoId !== videoId) {
                        queueHTML += `
                            <div style="display: flex; align-items: center; gap: 15px; padding: 8px 10px; border-radius: 5px; cursor: pointer; transition: background 0.2s;" 
                                 onmouseover="this.style.background='#282828'" onmouseout="this.style.background='transparent'"
                                 onclick="getStreamAndPlay('${nextVideoId}', '${nextTitle.replace(/'/g, "\\'")}', '${nextArtist.replace(/'/g, "\\'")}', '${nextThumb}')">
                                <img src="${nextThumb}" style="width: 40px; height: 40px; border-radius: 4px; object-fit: cover;">
                                <div style="flex: 1; overflow: hidden;">
                                    <div style="color: white; font-weight: 500; font-size: 13px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${nextTitle}</div>
                                    <div style="color: #b3b3b3; font-size: 12px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${nextArtist}</div>
                                </div>
                            </div>
                        `;
                        count++;
                    }
                }
            }
        });

        if (count === 0) queueHTML += `<p style="color: #888; font-size: 13px; text-align: center;">Không có bài hát tiếp theo.</p>`;
        queueBox.innerHTML = queueHTML;
    }

    // 4. GỌI API LẤY NHẠC VÀ LƯU LỊCH SỬ
    try {
        const response = await fetch(`http://thucpham111-001-site1.stempurl.com/api/youtubemusic/play/${videoId}`);
        if (!response.ok) throw new Error("Bài hát này bị chặn vùng hoặc đánh bản quyền!");
        const data = await response.json();
        
        const audioPlayer = document.getElementById('main-audio'); 
        if(audioPlayer) {
            audioPlayer.src = data.streamUrl;
            audioPlayer.play();
            if(document.getElementById('master-play')) document.getElementById('master-play').className = 'fas fa-pause-circle';
            
            const loggedInUser = localStorage.getItem('username'); 
            if (loggedInUser) {
                fetch('http://thucpham111-001-site1.stempurl.com/api/history', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        username: loggedInUser, videoId: videoId, title: title, artist: artist, thumbnail: thumbnail
                    })
                }).catch(err => console.error("❌ Lỗi lưu lịch sử:", err));
            }
        }
    } catch (error) {
        alert("Có lỗi: " + error.message);
    }
}


// ==========================================
// 4. KHỞI ĐỘNG CÁC NÚT BẤM KHI TRANG TẢI XONG
// ==========================================
document.addEventListener("DOMContentLoaded", function() {
    setupGlobalPopups();
    loadTrendingSongs(); 

    const audio = document.getElementById('main-audio');
    const playBtnIcon = document.getElementById('master-play');
    const progressFill = document.getElementById('progress-bar');
    const progressArea = document.getElementById('progress-container');
    const currentTimeText = document.getElementById('current-time');
    const totalTimeText = document.getElementById('duration');

    // Nút Play/Pause
    if (playBtnIcon && audio) {
        playBtnIcon.parentElement.addEventListener('click', () => {
            if (audio.paused) {
                audio.play();
                playBtnIcon.className = "fas fa-pause-circle"; 
            } else {
                audio.pause();
                playBtnIcon.className = "fas fa-play-circle"; 
            }
        });
    }

    // ==========================================
    // ĐIỀU KHIỂN THANH THỜI GIAN & KHẮC PHỤC LỖI 0:00 (INFINITY BUG)
    // ==========================================
    if (audio) {
        // 1. MẸO (HACK): Ép trình duyệt dò tìm thời lượng thật của bài hát
        audio.addEventListener('loadedmetadata', () => {
            if (audio.duration === Infinity || isNaN(audio.duration)) {
                audio.currentTime = 1e101; // Ép tua một phát đến "tận cùng vũ trụ"
                
                // Đợi trình duyệt dò xong, kéo nó về 0 để phát nhạc bình thường
                audio.addEventListener('timeupdate', function getRealDuration() {
                    this.removeEventListener('timeupdate', getRealDuration); 
                    audio.currentTime = 0; 
                    if (totalTimeText) totalTimeText.innerText = formatTime(audio.duration);
                });
            } else {
                if (totalTimeText) totalTimeText.innerText = formatTime(audio.duration);
            }
        });

        // 2. Chạy thanh màu xanh và đếm giây
        audio.addEventListener('timeupdate', () => {
            if(currentTimeText) currentTimeText.innerText = formatTime(audio.currentTime);
            
            if (audio.duration && !isNaN(audio.duration) && audio.duration !== Infinity) {
                const percent = (audio.currentTime / audio.duration) * 100;
                if (progressFill) progressFill.style.width = `${percent}%`;
                
                // Luôn cập nhật lại tổng thời lượng để chắc chắn nó không bị tụt về 0:00
                if (totalTimeText) totalTimeText.innerText = formatTime(audio.duration);
            }
        });

        // 3. Reset giao diện khi bài hát kết thúc
        audio.addEventListener('ended', () => {
            if (playBtnIcon) playBtnIcon.className = "fas fa-play-circle";
            if (progressFill) progressFill.style.width = '0%';
            if (currentTimeText) currentTimeText.innerText = "0:00";
        });
    }

    // Tua nhạc
    if (progressArea && audio) {
        progressArea.addEventListener('click', function(e) {
            // Thêm điều kiện chặn lỗi tua khi chưa có thời lượng thật
            if (audio.duration && !isNaN(audio.duration) && audio.duration !== Infinity) {
                const clickPercent = e.offsetX / this.clientWidth;
                audio.currentTime = clickPercent * audio.duration;
            }
        });
    }

    // Âm lượng
    const volumeSlider = document.getElementById('volume-slider');
    const volumeIcon = document.getElementById('volume-icon');
    if (volumeSlider && audio) {
        audio.volume = volumeSlider.value / 100;
        volumeSlider.addEventListener('input', function() {
            audio.volume = this.value / 100;
            if (volumeIcon) {
                if (audio.volume === 0) volumeIcon.className = 'fas fa-volume-mute';
                else if (audio.volume < 0.5) volumeIcon.className = 'fas fa-volume-down';
                else volumeIcon.className = 'fas fa-volume-up';
            }
        });
    }
    
    // KIỂM TRA ĐĂNG NHẬP (Tự động đổi nút Đăng nhập thành Xin chào)
    const loggedInUser = localStorage.getItem('username'); 
    if (loggedInUser) {
        const loginBtn = Array.from(document.querySelectorAll('a, button')).find(el => el.textContent.includes('Đăng nhập'));
        if (loginBtn) {
            const authContainer = loginBtn.parentElement;
            authContainer.innerHTML = `
               <i class="fas fa-user" style="color: #b3b3b3; margin-right: 10px; font-size: 18px;"></i>
                <span style="color: white; font-weight: bold; margin-right: 20px;">Xin chào, ${loggedInUser}!</span>
                <button onclick="localStorage.removeItem('username'); window.location.reload();" 
                        style="background: transparent; border: 1px solid white; color: white; padding: 5px 15px; border-radius: 20px; cursor: pointer;">
                    Đăng xuất
                </button>
            `;
        }
    }
});
// ==========================================
// 5. HÀM XỬ LÝ THẢ TIM / BỎ TIM BÀI HÁT
// ==========================================
async function toggleFavoriteSong(event, videoId, title, artist, thumbnail) {
    event.stopPropagation(); // Chặn click nhầm sang phát nhạc
    const heartIcon = event.target;
    heartIcon.style.color = "#888"; 

    try {
        const currentUser = localStorage.getItem('username');
        if (!currentUser) {
            alert("Bạn cần đăng nhập để lưu bài hát nhé!");
            window.location.href = "login.html";
            return;
        }

        const response = await fetch('http://thucpham111-001-site1.stempurl.com/api/youtubemusic/toggle-favorite', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                videoId: videoId, title: title, artist: artist, thumbnail: thumbnail, username: currentUser
            })
        });

        if (!response.ok) throw new Error("Lỗi API thả tim!");
        const data = await response.json();

        if (data.status === 'Liked') {
            heartIcon.className = "fas fa-heart";
            heartIcon.style.color = "#1db954"; 
        } else {
            heartIcon.className = "far fa-heart";
            heartIcon.style.color = "white"; 
        }
    } catch (error) {
        console.error("Lỗi:", error);
        alert("Không thể thả tim lúc này!");
        heartIcon.style.color = "white"; 
    }
}
// ==========================================
// 7. TỰ ĐỘNG TẠO VÀ XỬ LÝ POPUP TOÀN CẦU
// ==========================================
function setupGlobalPopups() {
    const btnNoti = document.getElementById('btn-noti');
    const btnFriend = document.getElementById('btn-friend');

    // Nếu trang nào không có thanh header (ví dụ trang đăng nhập), thì bỏ qua
    if (!btnNoti || !btnFriend) return;

    // 1. TỰ ĐỘNG VẼ BẢNG THÔNG BÁO VÀO HTML
    if (!document.getElementById('noti-panel')) {
        const notiHTML = `
            <div id="noti-panel" class="popup-panel" style="display: none;">
                <h4 style="margin-top: 0; color: white; border-bottom: 1px solid #333; padding-bottom: 10px;">Thông báo</h4>
                <ul style="list-style: none; padding: 0; margin: 0;">
                    <li class="popup-item">
                        <i class="fas fa-music" style="color: #1db954; font-size: 18px; width: 25px;"></i>
                        <div>
                            <b style="color: white; font-size: 13px;">H2T Music v2.0</b>
                            <p style="color: #b3b3b3; font-size: 11px; margin: 2px 0 0 0;">Giao diện siêu mượt đã hoàn thiện!</p>
                        </div>
                    </li>
                </ul>
            </div>
        `;
        btnNoti.insertAdjacentHTML('afterend', notiHTML);
    }

    // 2. TỰ ĐỘNG VẼ BẢNG BẠN BÈ VÀO HTML
    if (!document.getElementById('friend-panel')) {
        const friendHTML = `
            <div id="friend-panel" class="popup-panel" style="display: none;">
                <h4 style="margin-top: 0; color: white; border-bottom: 1px solid #333; padding-bottom: 10px;">Hoạt động bạn bè</h4>
                <ul style="list-style: none; padding: 0; margin: 0;">
                    <li class="popup-item">
                        <img src="https://i.pravatar.cc/100?img=11" style="width: 35px; height: 35px; border-radius: 50%;">
                        <div>
                            <b style="color: white; font-size: 13px;">Tuấn</b>
                            <p style="color: #b3b3b3; font-size: 11px; margin: 2px 0 0 0;">Đang nghe: Đánh Đổi - Obito</p>
                        </div>
                    </li>
                </ul>
            </div>
        `;
        btnFriend.insertAdjacentHTML('afterend', friendHTML);
    }

    // 3. GẮN SỰ KIỆN CLICK (BẬT/TẮT)
    const panelNoti = document.getElementById('noti-panel');
    const panelFriend = document.getElementById('friend-panel');

    btnNoti.addEventListener('click', (e) => {
        e.stopPropagation();
        panelNoti.style.display = panelNoti.style.display === 'none' ? 'block' : 'none';
        if (panelFriend) panelFriend.style.display = 'none'; // Tắt bạn bè đi
    });

    btnFriend.addEventListener('click', (e) => {
        e.stopPropagation();
        panelFriend.style.display = panelFriend.style.display === 'none' ? 'block' : 'none';
        if (panelNoti) panelNoti.style.display = 'none'; // Tắt thông báo đi
    });

    document.addEventListener('click', () => {
        if (panelNoti) panelNoti.style.display = 'none';
        if (panelFriend) panelFriend.style.display = 'none';
    });
    
    if (panelNoti) panelNoti.addEventListener('click', (e) => e.stopPropagation());
    if (panelFriend) panelFriend.addEventListener('click', (e) => e.stopPropagation());
}
// ==========================================
// 8. HỆ THỐNG TÌM KIẾM NHẠC (SEARCH ENGINE)
// ==========================================
document.addEventListener("DOMContentLoaded", function() {
    // 1. Dò tìm thanh tìm kiếm trên Header
    const searchInput = document.getElementById('search-input');
    
    if (searchInput) {
        searchInput.addEventListener('keypress', function(event) {
            // 2. Nếu khách gõ phím Enter
            if (event.key === 'Enter') {
                const query = this.value.trim(); 
                
                if (query) {
                    console.log("🔍 Đang gọi API tìm kiếm từ khóa:", query);
                    
                    // 3. Tạo một khu vực trống dưới Header để chứa kết quả (nếu chưa có)
                    let searchContainer = document.getElementById('search-results-area');
                    if (!searchContainer) {
                        searchContainer = document.createElement('div');
                        searchContainer.id = 'search-results-area';
                        searchContainer.style.padding = '0 30px'; 
                        
                        const topBar = document.querySelector('.top-bar');
                        if (topBar) {
                            topBar.parentNode.insertBefore(searchContainer, topBar.nextSibling);
                        }
                    }

                    // 4. Bật giao diện "Đang tải" cực ngầu
                    searchContainer.innerHTML = `
                        <div style="margin-top: 20px; text-align: left;">
                            <h2 style="color: white; margin-bottom: 20px;">Kết quả tìm kiếm cho: <span style="color: #1db954;">"${query}"</span></h2>
                            <p style="color: #b3b3b3;"><i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu từ máy chủ...</p>
                        </div>
                    `;

                    // 5. Alo cho Backend C# tìm nhạc
                    fetch(`http://thucpham111-001-site1.stempurl.com/api/youtubemusic/search?query=${encodeURIComponent(query)}`)
                        .then(res => res.json())
                        .then(data => {
                            if (!data || data.length === 0) {
                                searchContainer.innerHTML = `
                                    <div style="margin-top: 20px; text-align: left;">
                                        <h2 style="color: white;">Không tìm thấy kết quả nào cho "${query}"</h2>
                                        <p style="color: #b3b3b3;">Hãy thử nhập tên bài hát hoặc ca sĩ khác xem sao sếp nhé!</p>
                                    </div>
                                `;
                                return;
                            }

                            // 6. Vẽ danh sách bài hát vuông vức chuẩn Spotify
                            let html = `
                                <div style="margin-top: 20px; margin-bottom: 40px;">
                                    <h2 style="color: white; margin-bottom: 20px;">Kết quả tìm kiếm cho: <span style="color: #1db954;">"${query}"</span></h2>
                                    <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(180px, 1fr)); gap: 24px;">
                            `;
                            
                            data.forEach(song => {
                                const safeTitle = song.title ? song.title.replace(/'/g, "\\'") : "Đang cập nhật";
                                const safeArtist = song.artist ? song.artist.replace(/'/g, "\\'") : "H2T Music";
                                const thumbnail = song.thumbnail || 'logo.png'; 

                                html += `
                                    <div style="background: #181818; padding: 16px; border-radius: 8px; cursor: pointer; transition: background-color 0.3s ease;" 
                                         onmouseover="this.style.backgroundColor='#282828'" 
                                         onmouseout="this.style.backgroundColor='#181818'"
                                         onclick="getStreamAndPlay('${song.id}', '${safeTitle}', '${safeArtist}', '${thumbnail}')">
                                        <img src="${thumbnail}" style="width: 100%; aspect-ratio: 1; object-fit: cover; border-radius: 6px; margin-bottom: 16px; box-shadow: 0 8px 24px rgba(0,0,0,0.5);">
                                        <h4 style="color: white; font-size: 15px; margin: 0 0 8px 0; font-weight: 700; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${song.title}</h4>
                                        <p style="color: #b3b3b3; font-size: 13px; margin: 0; font-weight: 500; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">${song.artist}</p>
                                    </div>
                                `;
                            });
                            
                            html += `</div></div>`;
                            searchContainer.innerHTML = html; // Đổ giao diện ra màn hình
                        })
                        .catch(err => {
                            searchContainer.innerHTML = `<h3 style="color: red; margin-top: 20px; text-align: left;">❌ Lỗi kết nối Backend: ${err.message}</h3>`;
                        });
                }
            }
        });
    }
});