# Unicode 控制字元分類

- 問題背景：有時在處理資料時會遇到一些奇怪的字元導致處理異常，檢查後發現多是控制字元，因此想在前處理時將其移除
- 但其實正規式中的 \p{C} 包含多個分類，而 C# 中 char.IsControl 方法對應的是 \p{Cc} 類別

## 1. \p{Cc} 控制字元

- 基本控制字元：U+0000 至 U+001F
- DELETE：U+007F
- 擴充控制字元：U+0080 至 U+009F

## 2. \p{Cf} 格式字元

- 零寬度字元（不會顯示但會影響排版）
- 文字方向控制字元
- 常見格式字元：
  - ZWSP（Zero Width Space，零寬空格）：U+200B
  - ZWNJ（Zero Width Non-Joiner）：U+200C
  - ZWJ（Zero Width Joiner）：U+200D
  - LRM（Left-to-Right Mark）：U+200E
  - RLM（Right-to-Left Mark）：U+200F

## 3. \p{Cn} 未分配字碼

- Unicode 標準中尚未定義的字碼位置

## 4. \p{Co} 私人使用區

- 讓使用者自己定義用途的區域

## 5. \p{Cs} 代理對字碼

- 編碼 Unicode 第 1 平面以上的字元（U+10000 之後的字元）
- 用來表示更大範圍 Unicode 的特殊編碼機制，例如 emoji 表情符號

# 依據資料性質使用方法

```csharp
Regex.Replace(input, @"[\p{Cc}\p{Cf}\p{Cn}\p{Co}]", "");
```