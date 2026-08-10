# C# Aho-Corasick 多模式比對 Note

- 背景：在一個 ETL 系統當中，需要掃描大量裁判書全文，一一比對該裁判書是否涉及上市公司。我們的方案是沒有進行斷字斷詞，直接比對公司名稱字串

最直覺的做法是逐一比對，但每篇全文都要掃 k 次：

```csharp
// O(n × k)，k 為公司名稱數量
foreach (var companyName in companyNames)
{
    if (fullContent.Contains(companyName))
        matches.Add(companyName);
}
```

- 使用 Aho-Corasick 後單次掃描即可同時命中所有 pattern (在這個使用情境中 pattern 即為公司名稱)

改為預先建好結構，之後每篇全文只需掃一次：

```csharp
// 預處理
var aho = new AhoCorasick();
foreach (var companyName in companyNames)
    aho.Insert(companyName);
aho.Build();

// O(n + z)，n 為全文長度，z 為命中數
foreach (var match in aho.Search(fullContent))
    matches.Add(match);
```

## 演算法概念

將多個 pattern 預先建成 Trie + Fail 鏈，掃描文本時一次走完即可同時命中所有 pattern，複雜度 O(n + z)
- Trie 本體：將所有 pattern 共用前綴存成樹
- Fail 鏈：比對失敗時跳轉到最長可匹配後綴，避免回頭重掃
- Output 鏈：一個位置可能同時命中多個 pattern(含後綴)，沿 OutputLink 串接

## 何時使用

- pattern 數量多(數百筆以上)且固定不常變動
- 文本篇幅大、需要重複掃描多份
- ❌ pattern 少或文本短：直接 String.Contains 就好
- ❌ pattern 頻繁異動：每次都要重新 Build，成本是否划算還需要評估

## 專案實際使用範例

- 讀取資料：程式啟動後由資料庫讀取上市公司資料，包含名稱、產業別、股票代號等等，讀取出來後使用名稱作為 pattern 建立 Aho-Corasick 結構，之後比對都重複使用同一份(保留實體在記憶體中)

- 反查索引：Aho-Corasick 只會回傳命中了哪些字串，不包含該公司的股票代號、產業別等欄位。因此在建立 pattern 的同時，用同一份資料另建一份「名稱 → 完整資料」的 Lookup，命中後再由名稱反查

- 使用 `ILookup`：同一個公司名稱可能對應多筆資料，例如同時使用全名與別名比對的情境，`ILookup` 天然支援一對多且查無 key 時回傳空序列不必額外判斷


```csharp
private AhoCorasick _aho;
private ILookup<string, CompanyItem> _companyLookup;

private async Task EnsureAhoLoaded()
{
    if (_aho != null) return;

    List<CompanyItem> companies = await _db.GetCompanies();

    var aho = new AhoCorasick();
    foreach (CompanyItem item in companies)
        aho.Insert(item.Name);   // pattern 只放名稱

    // 同一份資料另建反查索引
    _companyLookup = companies.ToLookup(x => x.Name);
    _aho = aho;
}

public async Task<List<CompanyItem>> MatchFullContent(string fullContent)
{
    await EnsureAhoLoaded();

    var result = new List<CompanyItem>();
    foreach (string name in _aho.SearchDistinct(fullContent))
        result.AddRange(_companyLookup[name]);   // 由命中的名稱反查完整資料

    return result;
}
```

