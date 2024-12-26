/**
 * 圖片預覽輔助工具
 */
const imagePreviewer = (function () {
    let _canvas;
    const _img = new Image();
    const _reader = new FileReader();
    _reader.onload = (e) => { _img.src = e.target.result; };
    _img.onload = () => {
        _canvas.width = _img.width;
        _canvas.height = _img.height;
        _canvas.getContext('2d').drawImage(_img, 0, 0);
        _canvas.style.display = 'block';
    };
    const setCanvas = (canvas) => { _canvas = canvas; };
    const preview = (file) => { _reader.readAsDataURL(file); }
    return { setCanvas, preview };
})();

/**
 * 取得畫布資料
 */
function getCanvasData(canvas) {
    return canvas
        .getContext('2d')
        .getImageData(0, 0, canvas.width, canvas.height)
        .data;
}

/**
 * 取得畫布點選位置資料
 */
function getClickCanvasData(canvas, event) {
    const rect = canvas.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;
    return canvas.getContext('2d').getImageData(x, y, 1, 1).data;  // param: width = 1, height = 1
}

/**
 * Uint8ClampedArray 資料轉為 Base64Image
 */
function uint8ClampedArrayToBase64Image(data, width, height) {
    const newCanvas = document.createElement('canvas');
    try {
        const newCtx = newCanvas.getContext('2d');
        newCanvas.width = width;
        newCanvas.height = height;

        const newImageData = new ImageData(width, height);
        newImageData.data.set(data);
        newCtx.putImageData(newImageData, 0, 0);
        return newCanvas.toDataURL();
    }
    finally {
        newCanvas.remove();
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const previewCanvas = document.getElementById('canvas');
    const selectedColorInput = document.getElementById('selectedColor');
    const targetColorInput = document.getElementById('targetColor');
    const convertBtn = document.getElementById('convertBtn');
    const hueRotateInput = document.getElementById('hueRotateInput');
    const saturateInput = document.getElementById('saturateInput');
    const lightnessInput = document.getElementById('lightnessInput');

    imagePreviewer.setCanvas(previewCanvas);

    /**
     * imageInput change event handler
     * 在畫布顯示圖片
     */
    document.getElementById('imageInput').addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file)
            imagePreviewer.preview(file);
    });

    /**
     * previewCanvas click event handler
     * 將選取位置顏色設置給 selectedColorInput
     */
    previewCanvas.addEventListener('click', function (event) {
        const data = getClickCanvasData(previewCanvas, event);
        selectedColorInput.value = converter.RGBToHexColorCode(new RGB(data[0], data[1], data[2]));
        showConvertParams();
    });

    /**
     * selectedColorInput change event handler
     */
    selectedColorInput.addEventListener('change', function () {
        showConvertParams();
    });

    /**
     * targetColorInput change event handler
     */
    targetColorInput.addEventListener('change', function () {
        showConvertParams();
    });

    /**
     * convertBtn click event handler
     * 依據 selectedColorInput、targetColorInput 改變顏色，將結果在 outputImage 顯示
     */
    convertBtn.addEventListener('click', function () {
        const originalData = getCanvasData(previewCanvas);
        const convertedData = convertColor(originalData);
        document.getElementById('outputImage').src = uint8ClampedArrayToBase64Image(convertedData, previewCanvas.width, previewCanvas.height);
    });

    /**
     * 顯示轉換參數
     */
    function showConvertParams() {
        const selectedHSL = converter.HexColorCodeToHSL(selectedColorInput.value);
        const targetHSL = converter.HexColorCodeToHSL(targetColorInput.value);
        hueRotateInput.value = targetHSL.h - selectedHSL.h;
        saturateInput.value = targetHSL.s / selectedHSL.s;
        lightnessInput.value = targetHSL.l / selectedHSL.l;
    }

    /**
     * 轉換顏色
     */
    function convertColor(data) {
        const hueRotate = hueRotateInput.valueAsNumber;
        const saturate = saturateInput.valueAsNumber;
        const lightness = lightnessInput.valueAsNumber;
        for (let i = 0; i < data.length; i += 4) {
            let hsl = converter.RGBtoHSL(new RGB(data[i], data[i + 1], data[i + 2]));
            hsl.h = (hsl.h + hueRotate + 360) % 360;
            hsl.s = hsl.s * saturate;
            hsl.s = hsl.s > 100 ? 100 : hsl.s;
            hsl.l = hsl.l * lightness;
            hsl.l = hsl.l > 100 ? 100 : hsl.l;

            let newRGB = converter.HSLtoRGB(hsl);
            [data[i], data[i + 1], data[i + 2]] = [newRGB.r, newRGB.g, newRGB.b];
        }
        return data;
    }
});