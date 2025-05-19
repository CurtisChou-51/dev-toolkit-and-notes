# HTML dialog top layer

- HTML5 引入的 `<dialog>` 元素提供原生的對話框功能，透過 JavaScript 可使用 `.show()` 與 `.showModal()` 顯示

## show()

- 將 `<dialog>` 顯示在畫面上，但不會進入 **頂層渲染（top layer）**
- 不會自動加入灰色背景（backdrop）
- 不會阻止背景內容操作（非模態）
- 會受 `z-index` 控制，可能被其他非頂層元素遮蓋

### Example：
```html
<dialog id="myDialog">Content</dialog>

<script>
  document.getElementById("myDialog").show();
</script>
````


## showModal()

- 將 `<dialog>` 提升至瀏覽器的 **頂層渲染（top layer）**
- 自動添加半透明背景（backdrop）
- 模態效果：使用者無法與背景互動
- 不受一般 `z-index` 控制，其他非頂層元素無法蓋住它

### Example：
```html
<dialog id="myDialog">Content</dialog>

<script>
  document.getElementById("myDialog").showModal();
</script>
```

## 比較表

| 功能                     | `.show()` | `.showModal()` |
| ------------------------ | --------- | -------------- |
| 是否為模態（Modal）        | ❌        | ✅             |
| 是否加入 backdrop         | ❌        | ✅             |
| 可否被其他非頂層元素蓋住     | ✅       | ❌              |
| 是否阻止背景操作           | ❌        | ✅             |
| 支援 ESC 關閉             | ✅        | ✅             |


## 注意事項

- `<dialog>` 不支援所有舊版瀏覽器（如 IE）
- 使用 `.showModal()` 時，`<dialog>` 會進入 **top layer**，這是瀏覽器的特殊層級，**普通的 z-index 無法覆蓋**，如果有配合其他 UI 套版使用需考慮相容性
- 在我們遇到的一個情境中使用了 `.showModal()` 來顯示，因此需要覆蓋此 dialog 的 UI 也需要調整實作方法 
