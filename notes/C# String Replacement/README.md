# C# String Replacement

- 在一個報表生成情境中我們需要將模板 (Template) 中的標記 (Tag) 替換為實際值
  - Template：包含 Tag 的字串
  - Tag 格式固定：[英文大寫字母 + 數字]，例如 [C0012356]、[AB123]
  - 替換來源：來自資料庫或設定的 Dictionary<string, string>

## Example

- Template：
```
Dear customer [C0001], your order [O1234] has been shipped, and the estimated delivery time is [D2023].
```

- Dictionary：
```
var dictionary = new Dictionary<string, string>
{
    { "[C0001]", "Curtis" },
    { "[O1234]", "ORD-2023-001" },
    { "[D2023]", "2023-12-31" }
};
```

- Result：
```
Dear customer Curtis, your order ORD-2023-001 has been shipped, and the estimated delivery time is 2023-12-31.
```

## 實作方法

### (1) `string.Replace` 迴圈替換

```csharp
string ReplaceWithLoop(string template, Dictionary<string, string> dictionary)
{
    foreach (var kvp in dictionary)
    {
        template = template.Replace(kvp.Key, kvp.Value);
    }
    return template;
}
```

### 特點
- 程式碼簡單，無正規式開銷
- .NET 框架有優化，在多數情況下效能較好
- 每次 `Replace` 都可能產生新字串，記憶體開銷較大

### (2) `Regex.Replace` 一次性替換
```csharp
string ReplaceWithRegex(string template, Dictionary<string, string> dictionary)
{
    string pattern = @"\[[A-Za-z]+\d+\]";
    return Regex.Replace(template, pattern, match => 
        dictionary.TryGetValue(match.Value, out var value) ? value : match.Value
    );
}
```

### 特點
- 正規式編譯開銷
- 只需掃描模板一次，在字典大時效能較好
- 彈性處理未定義的標記

---

## 適用情境

| 情境	                    | string.Replace           | Regex.Replace      |
|---------------------------|--------------------------|--------------------|
| Small Template Dictionary | ✅ .NET有優化、無正規式開銷 | ⚠️ 正規式編譯開銷 |
| Large Template Dictionary | ⚠️ 效能差、多次分配記憶體 | ✅ 高效（一次性掃描） |


## 測試程式碼
- [測試程式碼](Program.cs)

```csharp
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // 測試資料
        var dictionary = GenerateDictionary(300);
        string template = GenerateTemplate(3000, dictionary.Keys);

        // test string.Replace
        var stopwatch = Stopwatch.StartNew();
        string resultLoop = ReplaceWithLoop(template, dictionary);
        stopwatch.Stop();
        Console.WriteLine($"string.Replace : {stopwatch.ElapsedMilliseconds} ms");

        // test Regex.Replace
        stopwatch.Restart();
        string resultRegex = ReplaceWithRegex(template, dictionary);
        stopwatch.Stop();
        Console.WriteLine($"Regex.Replace : {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine($"same: {resultLoop == resultRegex}");
    }

    static string ReplaceWithLoop(string template, Dictionary<string, string> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            template = template.Replace(kvp.Key, kvp.Value);
        }
        return template;
    }

    static string ReplaceWithRegex(string template, Dictionary<string, string> dictionary)
    {
        string pattern = @"\[[A-Za-z]+\d+\]";
        return Regex.Replace(template, pattern, match =>
            dictionary.TryGetValue(match.Value, out var value) ? value : match.Value
        );
    }

    static Dictionary<string, string> GenerateDictionary(int count)
    {
        var random = new Random();
        var dict = new Dictionary<string, string>();
        for (int i = 0; i < count; i++)
        {
            string key = $"[{(char)('A' + random.Next(26))}{random.Next(1000000):D6}]";
            dict[key] = $"Value_{i}";
        }
        return dict;
    }

    static string GenerateTemplate(int markerCount, IEnumerable<string> keys)
    {
        var random = new Random();
        var template = new StringBuilder();
        var keyList = new List<string>(keys);
        for (int i = 0; i < markerCount; i++)
        {
            for (int j = 0; j < 10; j++)
                template.Append(Guid.NewGuid());
            template.Append(keyList[random.Next(keyList.Count)] + " ");
            for (int j = 0; j < 10; j++)
                template.Append(Guid.NewGuid());
        }
        return template.ToString();
    }
}
```

---

## 進階優化
- 如果正規式較為複雜且多次重複使用，可以使用預編譯並快取 Regex

```csharp
private static readonly Regex _regex = new Regex(@"\[[A-Za-z]+\d+\]", RegexOptions.Compiled);

string ReplaceOptimized(string template, Dictionary<string, string> dictionary)
{
    return _regex.Replace(template, match => 
        dictionary.TryGetValue(match.Value, out var value) ? value : match.Value
    );
}
```