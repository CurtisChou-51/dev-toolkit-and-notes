# HTML button form 屬性應用

- HTML button form 屬性應用

```html
<form id="mainForm" action="/save">
    <input type="text" name="data">
</form>

<button type="submit" form="mainForm">提交</button>
```

## 核心特性：跨越結構限制

`form` 屬性允許將提交按鈕放置在頁面的任何地方，**不需位於 form 內部**。透過指定 `<form>` 的 `id`，使按鈕在點擊時能遠端觸發特定表單的提交。若按鈕同時位於 `<form>` 內且設定了 `form` 屬性，會優先提交屬性所指定的表單

## 用途
- 佈局乾淨：按鈕不再限制於表單當中，如：解決按鈕需固定在頂部導覽列，而表單內容在主區域的設計需求；也可避免為了提交功能而過度嵌套標籤影響佈局
- 表單控制：直接利用 HTML 原生行為達成跨元素互動，不需要撰寫Jacascript 存取 DOM，如：在同一個頁面定義多個隱藏表單，透過按鈕的 `form` 屬性觸發對應動作

## 其他進階提交屬性 (Form Overrides)

HTML5 允許按鈕在觸發提交時暫時複寫表單原有的設定，如：
- formaction：改變提交路徑，如：表單預設存檔，但點擊「預覽」按鈕時提交至不同的 URL
- formmethod：改變提交方法，如：表單預設使用 POST，但點擊「搜尋」按鈕時改用 GET 以便於 URL 分享
- formnovalidate：跳過瀏覽器的表單驗證

## references
- [MDN Web Docs - Button form attribute](https://developer.mozilla.org/en-US/docs/Web/HTML/Reference/Elements/button#attr-form)