# C# 字串資料預處理

## Unicode 控制字元分類

- 問題背景：有時在處理資料時會遇到一些奇怪的字元導致處理異常，檢查後發現多是控制字元，因此想在前處理時將其移除
- 但其實正規式中的 `\p{C}`（Other 其他字元）包含多個子分類。而 C# 中 `char.IsControl` 方法對應的僅是 `\p{Cc}` 類別，若直接過濾大類 `\p{C}`，可能會誤殺一些後續處理上需要保留的編碼，因此依子分類處理

### 1. \p{Cc} 控制字元

- 基本控制字元：U+0000 至 U+001F（C0 控制字元，如 Tab, LF, CR 等）
- DELETE：U+007F
- 擴充控制字元：U+0080 至 U+009F（C1 控制字元）

### 2. \p{Cf} 格式字元

- 零寬度字元（不會顯示但會影響排版）
- 文字方向控制字元
- 常見格式字元：
  - ZWSP（Zero Width Space，零寬空格）：U+200B
  - ZWNJ（Zero Width Non-Joiner）：U+200C
  - ZWJ（Zero Width Joiner）：U+200D
  - LRM（Left-to-Right Mark）：U+200E
  - RLM（Right-to-Left Mark）：U+200F
  - BOM（Byte Order Mark，位元組順序記號）：U+FEFF（讀取特定文字檔時易遇到，常造成不可見的字串比對失敗）
  - SHY（Soft Hyphen，軟性連字號）：U+00AD（網頁排版用，複製內容時偶爾會夾帶）

### 3. \p{Cn} 未分配字碼

- Unicode 標準中尚未定義的字碼位置

### 4. \p{Co} 私人使用區

- 讓使用者或軟體公司自己定義用途的區域（PUA，常用於自訂圖示字型）

### 5. \p{Cs} 代理對字碼

- **範圍為 U+D800 至 U+DFFF**
- 用途是編碼 Unicode 第 1 平面以上的字元（U+10000 之後的字元）
- 為 UTF-16 編碼保留。必須兩兩成對（High + Low Surrogate）才能表示更大範圍的特定字元，例如：**emoji 表情符號、部分 CJK 擴展區漢字**。單獨出現時沒有實質意義。

---

## 依據資料性質使用方法

若想清除所有（常見）不具實質內容的控制與格式字元：

```csharp
// 這個過濾方法不將 \p{Cs} 加入，避免破壞字串中的 Emoji 符號或罕見中文字
Regex.Replace(input, @"[\p{Cc}\p{Cf}\p{Cn}\p{Co}]", "");
```

> [!NOTE]  
> 雖然 `\p{Z}` 系列（如 `\p{Zp}` 段落分隔 U+2029、`\p{Zl}` 行分隔 U+2028）不屬於 `\p{C}` 控制大類，但若資料清理目標是包含「不可見的干擾排版字元」，實務上也可視情況一併處理。

---

## 字串比對與正規化陷阱 (ToLowerInvariant)

在處理外部輸入資料字串時，除了過濾掉不可見的控制字元外，另一個常見的陷阱是**大小寫轉換的文化特性 (Culture)** 問題。

在 C# 中，如果直接使用 `.ToLower()` 或 `.ToUpper()`，預設會依照**當前執行緒的語系文化 (Current Culture)** 進行轉換。這在某些語系下會產生非預期的結果：

- 範例 Turkish-I Problem：
  在土耳其語系（`tr-TR`）中，大寫的英文 `I` 轉小寫會變成無點的 `ı`（U+0131），而不是一般英文預期的 `i`（U+0069）

### 何時使用

不是顯示給使用者的本地化內容，而需要統一文字大小寫的狀況如：
- 程式邏輯判斷
- 後端辨識碼轉換
- 建立不區分大小寫的雜湊

建議使用 `ToLowerInvariant()` 或 `ToUpperInvariant()`

```csharp
// 若是為了單純判斷相等，直接使用 StringComparison 效能更好（不產生額外字串配置）
if (string.Equals(input, "admin", StringComparison.OrdinalIgnoreCase)) {

}
```
