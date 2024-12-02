# IEnumerable 遍歷效能問題 Troubleshooting

- 背景場景：.NET MVC 系統某個 Razor 頁面產生速度慢，但其對應的資料查詢速度正常

## 功能架構

- 使用 ADO.NET 從資料庫讀取資料到 DataTable
- 使用反射機制將 DataTable 轉換為 ViewModel
- 利用 Razor 引擎將 ViewModel 渲染為 HTML 頁面

## 示意程式

- 反射方法
```csharp
public static IEnumerable<T> DataTableToModels<T>(DataTable dt) where T : new()
{
    PropertyInfo[] properties = typeof(T).GetProperties();
    foreach (DataRow row in dt.Rows)
    {
        T item = new T();
        foreach (PropertyInfo property in properties)
        {
            if (!dt.Columns.Contains(property.Name))
                continue;
            object value = row[property.Name];
            if (value != DBNull.Value)
            {
                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                property.SetValue(item, Convert.ChangeType(value, convertTo));
            }
        }
        yield return item;
    }
}
```

- ViewModel
```csharp
public class XXXViewModel
{
    public IENumerable<XXXResultModel> SearchResult { get; set; }
}
```

- Controller
```csharp
DataTable data = GetData();  // ADO.NET 讀取資料
model.SearchResult = DataTableToModels<XXXResultModel>(data);  // 反射將 DataTable 轉為 ViewModel
```

- Razor
```csharp
@for (int i = 0; i < Model.SearchResult.Count(); i++)
{
    <tr>
        <td>@i</td>
        <td>@Model.SearchResult.ElementAt(i).ColA</td>
        <td>@Model.SearchResult.ElementAt(i).ColB</td>
    </tr>
}
```

## 問題診斷

- 可以看到問題出在 Razor 頁面渲染時的集合遍歷方式：

1. 使用 `ElementAt(i)` 導致每次存取元素時都需重新遍歷集合，原本 O(n) 的遍歷變成 O(n²)
2. 由於 IEnumerable 的延遲執行特性，加上渲染之前沒有呼叫 `ToList()`，導致每次遍歷都會重新執行反射轉換

## 處理方法

- 在不更動現有架構的前提下採用以下優化：

1. 使用 foreach 遍歷，不需要使用 `ElementAt(i)` 存取
2. 在反射轉換後立即呼叫 `ToList()` 確保集合具體化
