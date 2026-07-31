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