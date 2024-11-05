/**
 * 驗證 FormData 中的所有檔案是否可以存取
 * @param {FormData} formData - 要驗證的表單資料
 * @returns {Promise<{isValid: boolean, errors?: Array}>} 驗證結果
 */
function validateFormDataFileAccessible(formData) {
    // 如果不是 FormData 物件，直接回傳成功
    if (!formData instanceof FormData)
        return Promise.resolve({ isValid: true });
    const errors = [];
    const validationPromises = [];
    for (const [fieldName, value] of formData.entries()) {
        if (!(value instanceof File))
            continue;
        validationPromises.push(
            validateFileAccessible(value).catch(error => {
                errors.push({ field: fieldName, file: value.name, message: error.message });
            })
        );
    }
    // 如果沒有需要驗證的檔案，直接回傳成功
    if (!validationPromises.length)
        return Promise.resolve({ isValid: true });
    return Promise.all(validationPromises)
        .then(() => ({ isValid: errors.length === 0, errors: errors }));
}

/**
 * 驗證單一檔案是否可以存取
 * @param {File} file - 要驗證的檔案物件
 * @returns {Promise} 如果檔案可以存取則 resolve，否則 reject
 */
function validateFileAccessible(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => { resolve(true); };
        reader.onerror = () => { reject(new Error(`檔案 "${file.name}" 無法存取，請檢查是否被刪除或移動`)); };
        try {
            reader.readAsArrayBuffer(file.slice(0, 1));
        }
        catch (error) {
            reject(new Error(`檔案 "${file.name}" 讀取錯誤: ${error.message}`));
        }
    });
}