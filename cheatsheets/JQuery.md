# JQuery

## make plugin intellisense
- for Visual Studio
```javascript
// 只給 IDE Intellisense 用，不要引入執行
interface JQuery {
    /**
     * form serializeArray 並用 extraData 覆蓋同名欄位
     * @param extraData - 額外資料
     * @returns 回傳序列化後的陣列
     */
    serializeArrayExt(extraData?: object): Array<{ name: string; value: any }>;
}
```