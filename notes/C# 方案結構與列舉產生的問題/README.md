# C# 方案結構與列舉產生的問題

## 概述

- 在一個 Web 專案中，我們定義了一些列舉類型來表示不同的狀態，例如 `PersonQueryType` 列舉被定義在 `EnumConstants.cs` 檔案中

```csharp
public enum PersonQueryType
{
    ID,
    Name
}
```

## 問題描述

- 在另一個 Console 專案中也使用了 `PersonQueryType` 列舉，但是該 Console 專案沒有加入到主方案中，此專案對於列舉的引用方式是由 Web 專案複製了一份 `EnumConstants.cs` 檔案
- 在修改 Web 專案的 `PersonQueryType` 列舉時，兩個專案中的定義可能會不一致導致問題。例如在 Web 專案中新增了 `Email` 選項，Console 專案中未同步更新
- 且 `Email` 選項新增的位置在中間，加上 `PersonQueryType` 沒有明確指定值，導致 `Name` 的值改變了。而 Web 專案會將列舉值轉為 `int` 之後提供 Console 專案使用，導致執行時出現非預期結果

```csharp
public enum PersonQueryType
{
    ID,
    Email,  // add Email
    Name
}
```

## 修改建議

1. **方案結構調整**：將另一個 Console 專案也應加入到主方案中，以便更好地管理和同步更新
2. **提取類別庫**：將 `EnumConstants.cs` 由 Web 專案提取到一個類別庫專案中，並在 Web 與 Console 專案中引用該類別庫，這樣可以確保列舉定義的一致性
3. **列舉定義調整**：在定義列舉時，建議為每個列舉成員明確指定值，以避免在未來修改時引入潛在的問題
4. **列舉修改注意**：在修改沒有明確指定值的列舉時，應注意新增成員的位置，以避免影響現有成員的值。例如應將新成員新增到列舉的末尾，以確保現有成員的值不會改變

```csharp
public enum PersonQueryType
{
    ID = 0,
    Name = 1,
    Email = 2
}
```
