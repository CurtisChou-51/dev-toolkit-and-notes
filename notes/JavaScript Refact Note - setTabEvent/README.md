# JavaScript Refact Note - setTabEvent

- 對於按鈕與頁籤功能的重構筆記

## 重構描述

- 流程控制：
  - 主流程簡潔，只負責事件註冊
  - 將事件處理邏輯 `listener` 抽離為獨立函數
  - 提取函數配合 `early return` 減少嵌套層級
  
- 效能優化：
  - 對於 `NodeList` 可以直接 `for loop` 處理，減少不必要的陣列轉換
  - 對於 `addEventListener` 的調整，從每個按鈕一個 `listener` 實體改為所有按鈕共用一個 `listener` 實體，可減少記憶體使用
  
## Before

```javaScript
function setTabEvent() {
    let tabBtn = document.querySelectorAll(`[tab-name][tab-btn]`);
    Array.from(tabBtn).forEach((btnElement) => {
        if (checkTargetEvent(btnElement, 'auto-tab-event')) {
            btnElement.addEventListener('click', (event) => {
                event.preventDefault();
                let tabName = event.currentTarget.getAttribute('tab-name'),
                    tabIndex = event.currentTarget.getAttribute('tab-btn'),
                    tabBtnFamily = [],
                    tabTargetFamily = [];
                if (tabName != undefined) {
                    tabBtnFamily = document.querySelectorAll(`[tab-name="${tabName}"][tab-btn]`)
                    Array.from(tabBtnFamily).forEach((updateBtnElement) => {
                        updateBtnElement.classList.remove('Focus')
                    });
                    event.currentTarget.classList.add('Focus');
                    tabTargetFamily = document.querySelectorAll(`[tab-name="${tabName}"][tab-target]`)
                    Array.from(tabTargetFamily).forEach((updateBtnElement) => {
                        let targetIndex = updateBtnElement.getAttribute('tab-target');
                        if (targetIndex != undefined && targetIndex == tabIndex)
                            updateBtnElement.style.display = '';
                        else
                            updateBtnElement.style.display = 'none';
                    });
                }
            })
        }
    });
}
```

## After

```javaScript
function setTabEvent() {
    let tabBtns = document.querySelectorAll(`[tab-name][tab-btn]`);
    for (let btnElement of tabBtns) {
        if (!checkTargetEvent(btnElement, 'auto-tab-event'))
            continue;
        btnElement.addEventListener('click', tabBtnClickEvt);
    };
}

function tabBtnClickEvt(event) {
    event.preventDefault();
    let tabName = event.currentTarget.getAttribute('tab-name'),
        tabIndex = event.currentTarget.getAttribute('tab-btn');
    if (!tabName)
        return;

    let tabBtnFamily = document.querySelectorAll(`[tab-name="${tabName}"][tab-btn]`);
    for (let updateBtnElement of tabBtnFamily)
        updateBtnElement.classList.remove('Focus');
    event.currentTarget.classList.add('Focus');

    let tabTargetFamily = document.querySelectorAll(`[tab-name="${tabName}"][tab-target]`);
    for (let updateBtnElement of tabTargetFamily) {
        let targetIndex = updateBtnElement.getAttribute('tab-target');
        updateBtnElement.style.display = targetIndex == tabIndex ? '' : 'none';
    }
}
```