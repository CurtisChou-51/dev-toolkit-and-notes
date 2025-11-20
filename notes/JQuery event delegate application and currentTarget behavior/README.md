# JQuery event delegate application and currentTarget behavior

- 這個範例背景是 ASP.NET Core MVC 網頁，不同於現在較流行的前端框架 render DOM，範例網頁 DOM 元素是透過後端 render 動態產生

- 這樣的背景下，我們會使用 JQuery 事件委派 (event delegation) 來處理網頁上的事件，並與 ES6 Class 結合，例如：

```javascript
this.panel.on('click', '[my-click]', (e) => {
    const $target = $(e.currentTarget);
    const funcName = $target.attr('my-click');
    const func = this[funcName];
    if (typeof func === 'function')
        func.call(this, $target);
});
```

## 範例

html：

```html
<button my-click="edit">
<button my-click="remove">
```

對應 javascript：

```javascript
edit($btn) { ... }
remove($btn) { ... }
```

- 這樣的事件委派方式有幾個好處：
  - 動態加入的 DOM 自動擁有事件，不需要重新綁定
  - 只需綁定一個事件處理器，比起逐一綁定減少記憶體使用
  - 透過這種方式綁定事件的 DOM 在刪除時不需要手動解除事件
  - 事件綁定在程式碼中集中管理，事件邏輯更直覺、較易維護