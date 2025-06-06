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