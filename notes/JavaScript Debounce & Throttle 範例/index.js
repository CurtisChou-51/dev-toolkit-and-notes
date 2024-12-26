function debounce(func, delay) {
    let timer = null;
    return function (...args) {
        let context = this;
        clearTimeout(timer);
        timer = setTimeout(function () {
            func.call(context, ...args)
        }, delay);
    }
}

function throttle(func, delay) {
    let timer = null;
    return function (...args) {
        if (timer)
            return;
        let context = this;
        timer = setTimeout(() => {
            timer = null;
        }, delay);
        func.call(context, ...args);
    };
}
document.addEventListener("DOMContentLoaded", (event) => {
    let msgDiv = document.getElementById('Message');
    let i = 0;
    let oriClick = function(message) {
        msgDiv.innerHTML += `${message}<br>`;
    };
    
    let click_throttle = throttle(oriClick, 1000);
    document.getElementById('showBtn_throttle').onclick = function() {
        i++;
        click_throttle(`click_throttle ${i}`);
    }
    
    let click_debounce = debounce(oriClick, 1000);
    document.getElementById('showBtn_debounce').onclick = function() {
        i++;
        click_debounce(`click_debounce ${i}`);
    }
});
