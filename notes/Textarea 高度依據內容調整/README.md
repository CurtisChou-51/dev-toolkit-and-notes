# Textarea 高度依據內容調整

- 背景：頁面上有些 textarea，進入頁面時將所有 textarea 高度顯示為與內容相符

## CSS 方案

- 現代 CSS 提供了 `field-sizing: content` 屬性，能根據內容自動調整高度，然而 `field-sizing` 屬性是一個相對較新的 CSS 特性，目前仍是實驗性質，尚未得到所有瀏覽器的完全支援
- 目前 Visual Studio 也標記不是個已知的 CSS 屬性  
![01.png]

## 解決方案

- 仍然套用 CSS 方案的設置
- 使用 `CSS.supports()` 方法檢測瀏覽器是否支援 `field-sizing: content` 屬性
- 對於不支援的瀏覽器，使用 JavaScript 實現動態調整 textarea 高度的功能

1. CSS
```css
textarea.fit-content-vertical {
    height: auto;
    resize: vertical;
    field-sizing: content;
}
```

2. JavaScript
```javascript
if (!CSS.supports('field-sizing', 'content')) {
    $panel.find('textarea').each(function() {
        fixTextareaHeight(this);
    });
}

function fixTextareaHeight(textarea) {
    textarea.style.height = '1px';
    textarea.style.height = (textarea.scrollHeight + 3) + 'px';
}
```

## 參考
- [javascript - Creating a textarea with auto-resize - Stack Overflow](https://stackoverflow.com/questions/454202/creating-a-textarea-with-auto-resize)
- [javascript - Textarea Auto height - Stack Overflow](https://stackoverflow.com/questions/17772260/textarea-auto-height)