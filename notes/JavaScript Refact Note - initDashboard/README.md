# JavaScript Refact Note - initDashboard

- 對於 `initDashboard` 的重構筆記，處理依據不同部門與設定的使用者看到不同儀表板的需求情境，主要手法為將決策點上提到進入點

## 重構描述

- 決策點上提：
  - `isDeptA` 原本在 `initDashboard` 與 `loadDashboard` 的 callback 內各判斷一次，同一個條件散在兩個層級，上提後只在進入點判斷一次
  - `isDeptA` 的判斷在進入 `loadDashboard` 之前就確定了，直接交由呼叫端處理，不需往下滲透

- guard clause：`initDashboard` 改為 early return 取代 if-else

- 移除不使用參數：DeptA 路徑從未使用 `index` 但原版強迫傳入。拆分後 `loadDashboard_DeptA()` 直接不收參數

### 決策點重構訊號

- 同一條件在不同層級判斷兩次
- 函式內讀取外部旗標決定行為
- 分支埋在非同步 callback 內
- 參數只為了某條路徑而存在


## Before

```javascript
function initDashboard() {
    if (isDeptA || useNormal)
        loadDashboard(idx);
    else
        loadDashboard_Simplified();
}

// 部門 B 的使用情境下會在其他地方呼叫，傳入 index 參數
function loadDashboard(index) {
    post('/api/dashboard', { index: index }, function (data) {
        if (isDeptA)
            renderDeptA(data);
        else
            renderDeptB(data, index);
    });
}
```

## After

```javascript
function initDashboard() {
    if (isDeptA)
        return loadDashboard_DeptA();
    if (useNormal)
        return loadDashboard_DeptB(idx);
    return loadDashboard_Simplified();
}

function loadDashboard_DeptA() {
    post('/api/dashboard', {}, renderDeptA);
}

// 部門 B 的使用情境下會在其他地方呼叫，傳入 index 參數
function loadDashboard_DeptB(index) {
    post('/api/dashboard', { index: index }, d => renderDeptB(d, index));
}
```
