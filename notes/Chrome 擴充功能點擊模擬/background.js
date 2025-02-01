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