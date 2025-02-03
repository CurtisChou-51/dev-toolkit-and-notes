# Chrome 擴充功能點擊模擬

- 在 Chrome 擴充功能可使用 Debugger API 模擬在網頁上的 isTrusted 點擊事件


## manifest 配置

- 在 `manifest.json` 中加入必要權限，並在 Background Script 中使用

```json
{
  "permissions": [
    "tabs", "debugger"
  ],
  "background": {
    "service_worker": "background.js"
  }
}
```

## Background Script

- 在 Background Script 定義 function

```javaScript
async function clickAt(tabId, x, y) {
    await chrome.debugger.attach({ tabId: tabId }, "1.3");
    await chrome.debugger.sendCommand({ tabId: tabId }, 
        "Input.dispatchMouseEvent", 
        {
            type: "mousePressed",
            x: x,
            y: y,
            button: "left",
            clickCount: 1
        });
    await chrome.debugger.sendCommand({ tabId: tabId }, 
        "Input.dispatchMouseEvent", 
        {
            type: "mouseReleased",
            x: x,
            y: y,
            button: "left",
            clickCount: 1
        });
    await chrome.debugger.detach({ tabId: tabId });
}
```