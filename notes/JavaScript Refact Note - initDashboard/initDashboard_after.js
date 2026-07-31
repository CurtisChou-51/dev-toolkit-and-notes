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