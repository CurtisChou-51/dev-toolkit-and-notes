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