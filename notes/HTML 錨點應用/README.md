# HTML 錨點應用

- HTML 錨點(Anchor) 應用

## 核心特性

- **不會傳送到伺服器**：URL 中 `#` 及其後方的內容(Fragment Identifier)僅保留在瀏覽器端，不會包含在 HTTP 請求中
- **不會導致頁面重新整理**：改變錨點值會觸發瀏覽器的歷史紀錄變化，但不會重新向伺服器請求資源，適合用於單頁應用切換

## 常見用途

### 定位
- **ID 定位**：瀏覽器會尋找 HTML 中 `id` 屬性相符的元素並捲動至該處，常用於頁內導覽如 API 文件
- **文字片段定位 (Scroll to Text Fragment)**：Chromium 系瀏覽器支援的新語法，除了定位外還可直接 HighLight 特定文字

```
傳統 ID 定位
https://example.com/page#section1

文字片段定位 (語法為 #:~:text=)
https://example.com/page#:~:text=keyword
```

### CSS 應用
- 利用 CSS 的 `:target` 選擇器，可以在不使用 JavaScript 的情況下針對當前被選中的錨點目標改變樣式，常見於頁籤切換

```css
/* 隱藏所有分頁內容 */
.tab-pane {
    display: none;
}

/* 當 id 與錨點匹配時顯示 */
.tab-pane:target {
    display: block;
    animation: fadeIn 0.5s ease;
}
```

### 其他
部分瀏覽器支援的錨點用法：

- **多媒體片段 (Media Fragments)**：在影音 URL 中加入時間參數，指定播放起點與終點
```
video.mp4#t=10,20
```

- **PDF 頁碼定位**：在 PDF 文件 URL 中加入頁碼參數，直接跳轉至指定頁面
```
file.pdf#page=5
```

## 監聽錨點變化

Javascript 可利用 `hashchange` 事件監聽

```javascript
// 監聽錨點變化
window.addEventListener('hashchange', function() {
    console.log(window.location.hash);
});
```

## 使用總結

錨點技術因其「不刷新頁面」與「僅留於客戶端」的特性，廣泛應用於以下情境：

- **長網頁導覽**：
用於 API 文件、維基百科或法律條款，點擊目錄後直接跳轉至對應的 `id` 區塊

- **精確資訊分享**：
透過 `#:~:text=` 或 `page=5` (PDF)，將他人引導至網頁中特定的文字段落或文件頁碼，而非整個頁面頂端

- **Pure CSS UI**：
利用 `:target` 偽類實作分頁標籤 (Tabs)、彈出視窗 (Lightbox) 或收納選單，減少對 JavaScript 的依賴

- **單頁應用路由**：
在不具備後端路由支援的環境下，利用 `hashchange` 監聽 `#` 變化來切換前端組件內容

- **多媒體標記**：
在分享影片連結時，利用 `#t=time` 指定起點，讓閱聽者直接觀看重點片段
