javascript:(function(){

    let textDom = document.getElementsByClassName("Icaptcha")[0];
    let imgDom = document.getElementsByClassName("captchaImg")[0];
    let clonedImgDom = document.getElementById("_cloneImg") ?? imgDom.cloneNode(true);
    clonedImgDom.id= "_cloneImg";
    imgDom.insertAdjacentElement('afterend', clonedImgDom);

    /* 刷新驗證碼 (updateCaptcha()是既有function) */
    async function reloadCaptcha() {
        return new Promise((resolve, reject) => {
            imgDom.onload = function() { resolve(); };
            updateCaptcha();
        });
    }

    /* 載入js */
    async function loadScript(url) {
        return new Promise((resolve, reject) => {
            if (window.Tesseract) {
                resolve("loaded");
                return;
            }
            let script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = url;
            script.onload = () => resolve(script);
            script.onerror = () => reject(new Error(`Script load error for ${url}`));
            document.head.appendChild(script);
        });
    }

    /* 將圖片簡單二值化 */
    function binarizedImage(targetColor, threshold) {
        let canvas = document.createElement('canvas');
        canvas.width = imgDom.naturalWidth;
        canvas.height = imgDom.naturalHeight;
        let ctx = canvas.getContext('2d');
        ctx.drawImage(imgDom, 0, 0);

        let imageDataObj = ctx.getImageData(0, 0, canvas.width, canvas.height);
        let data = imageDataObj.data;

        for (let i = 0; i < data.length; i += 4) {
            let [r, g, b] = [data[i], data[i + 1], data[i + 2]];
            if (Math.abs(r - targetColor[0]) < threshold &&
                Math.abs(g - targetColor[1]) < threshold &&
                Math.abs(b - targetColor[2]) < threshold) {
                data[i] = data[i + 1] = data[i + 2] = 0;
            }
            else {
                data[i] = data[i + 1] = data[i + 2] = 255;
            }
        }

        ctx.putImageData(imageDataObj, 0, 0);
        return canvas.toDataURL();
    }  

    /* 處理文字(已知此驗證碼只有小寫英文與數字，可由此作一些簡單處理) */
    function processText(text) {
        let charMap = {
            'A': '4', 'B': '8', 'C': '6', 'D': '0', 'E': '3', 'F': '7', 
            'G': '6', 'H': 'h', 'I': '1', 'J': 'j', 'K': 'k', 'L': '1', 
            'S': '5', 'O': '0', 'T': 'l', 'Y': '9', 'Z': '2'
        };
        for (let [upper, replacement] of Object.entries(charMap)) {
            text = text.replace(new RegExp(upper, 'g'), replacement);
        }
        text = text.replace(/[^a-z0-9]/g, '');
        return text;
    }

    /* 將symbols轉為文字，若結果可信度太低或是有明顯錯誤則回傳空字串 */
    function symbolsToText(symbols) {
        let validSymbolRegex = /^[a-zA-Z0-9]$/;
        let validSymbols = symbols.filter(symbol => 
            validSymbolRegex.test(symbol.text) && 
            symbol.text.length === 1 && 
            symbol.confidence >= 85
        );
        let result = (validSymbols.length == 5) ? processText(validSymbols.map(x => x.text).join("")) : "";
        return result.length == 5 ? result : "";
    }

    /* 執行OCR並回傳處理後字串 */
    async function tryOCR() {
        let binarizedBase64 = binarizedImage([0, 0, 255], 180); 
        clonedImgDom.src = binarizedBase64;

        return Tesseract.recognize(
            binarizedBase64,
            'eng',
            {
                logger: m => console.log(m),
                tessedit_char_whitelist: '0123456789abcdefghijklmnopqrstuvwxyz',
                tessedit_pageseg_mode: Tesseract.PSM.SINGLE_LINE
            }
        ).then(result => {
            let symbols = result.data.symbols;
            console.log(symbols);
            return symbolsToText(symbols);
        });
    }

    /* 主程式，不斷刷新驗證碼並嘗試OCR直到產生較可信的結果 */
    (async function(){
        await loadScript("https://unpkg.com/tesseract.js@v2.1.0/dist/tesseract.min.js");
        while(1) {
            await reloadCaptcha();
            let text = await tryOCR();
            if (text.length) {
                textDom.value = text;
                textDom.dispatchEvent(new Event('input', { bubbles: true }));
                break;
            }
        }
    })();
})();

