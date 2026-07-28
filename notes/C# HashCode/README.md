# C# HashCode

## 背景

- 最近在舊專案中看到類似這樣的操作：
```csharp
int key = HashCode.Combine(dto.Type, dto.Deptid, dto.Year);
dict[key] = dto;
```

雖然能看出這段程式的意圖，但感覺有些怪所以先理解了下 `GetHashCode`

## 效果與用途
- 將一個物件快速對應到 int (可能會碰撞)，讓 `Dictionary`、`HashSet`、`Lookup` 這些基於雜湊表機制的資料結構可以使用 `GetHashCode` + `Equals` 查找

### 查找機制
- 以 `Dictionary` 為例，實現上是由兩部分組成：Buckets (桶子) 與 Entries (資料)，當要比對一個 key 是否有在 `Dictionary` 時：
  1. 先由 key 的 `GetHashCode` 算出要放在哪個桶子 (找 bucket index)，還會搭配 MOD 運算
  2. 因為可能發生碰撞，桶子內會有其他資料，這時對該桶子內資料逐一使用 `Equals` 與輸入資料比對

- 如此藉由 bucket index 快速定位，在一般狀況下 (碰撞不嚴重) 可以達到近乎 `O(1)` 的查找

### 與 Equals 關係
- 如果 override 了 `Equals`，還必須重寫 `GetHashCode`，才能確保分到同一桶子
- 遵循兩個定律：
  - 如果 `A.Equals(B)`，則兩者 `GetHashCode` 結果相同
  - 如果 `GetHashCode` 結果相同，未必 `A.Equals(B)`，還需要二階段比對

## 注意事項
- **不要當唯一識別使用**：像是案例中的用法就不正確因為會有碰撞，除非還要自己實現 Buckets + Entries
- **不要持久化儲存**：每次重啟對於相同的輸入都可能會算出不同結果，儲存並沒有意義

## 正確用途
`HashCode.Combine` 是設計給 **override `GetHashCode()`** 時用的，不是拿來當獨立的 key。這個情境中改用 value tuple `(dto.Type, dto.Deptid, dto.Year)` 作為 key (tuple 的 equality 是逐欄比對)

> [!NOTE]  
> Hash code 是快速定位的門牌號碼，並非唯一識別；門牌號碼相同不代表個體相同，還要開門確認(`Equals`)