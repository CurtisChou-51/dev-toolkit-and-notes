# JavaScript FileList Note

- FileList 是一個類陣列(array-like)的物件，通常由檔案輸入元件 `<input type="file">` 或拖放(drag、drop)操作產生，FileList 裡面包含一個或多個 File 物件，每個 File 物件代表一個使用者選擇的檔案
- FileList 是唯讀的，不能直接 new FileList() 或修改其內容

## 在網頁取得 FileList

- 使用者操作(選擇、拖放)時可以取得 FileList，瀏覽器不允許 javascript 任意讀取本機檔案

1. 透過 input 元素的 files 屬性
```javascript
// HTML: <input type="file" id="uploadFiles" multiple>

const files = document.getElementById('uploadFiles').files;  // files
```

2. 透過拖放事件 (Drag and Drop)
```javascript
const dropZone = document.getElementById('dropZone');

dropZone.addEventListener('drop', function(event) {
    event.preventDefault();
    const files = event.dataTransfer.files;  // files
});

dropZone.addEventListener('dragover', function(event) {
    event.preventDefault();
});
```

## Binding - 以 ASP.NET Core MVC 為例

