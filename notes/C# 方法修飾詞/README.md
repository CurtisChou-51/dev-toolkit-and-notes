# C# 方法修飾詞
- 複習 C# abstract、virtual、override、new 方法修飾詞與多型

| 修飾詞 | 使用於 | 需要實作 | 特色 | 
|--------|--------|-----------|------|
| `abstract` | 父類別 | 否 | - 類別必須宣告為 abstract<br>- 隱含 virtual<br>- 子類別必須實作 (除非子類別也是 abstract) |
| `virtual` | 父類別 | 是 | - 提供預設實作<br>- 允許子類別選擇性覆寫 |
| `override` | 子類別 | 是 | - 覆寫父類別的 virtual 或 abstract 方法<br>- 必須完全符合父類別方法簽章 |
| `new` | 子類別 | 是 | - 隱藏父類別方法<br>- 不需要父類別方法是 virtual<br>- 會產生警告表示隱藏了父類別方法 |

## abstract 抽象方法
- 父類別使用
- 表示此方法還沒被實作
- 如果一個方法為 abstract，該類別也要一併宣告成 abstract
- 此方法可被子類別 override (隱含 virtual)，如果子類別不是 abstract 則必須 override

## virtual 虛擬方法
- 父類別使用
- 此方法可被子類別 override

## override 覆寫方法
- 子類別使用
- 用於覆寫父類別的 virtual 或 abstract 方法
- 方法的選擇根據**物件的執行期類型**，會保有多型性

## new 隱藏方法
- 子類別使用
- 父類別方法沒有 virtual 也可以被子類別 new 隱藏
- 語法與效果上類似 override，但意義不相同
- 方法的選擇根據**物件的編譯期類型**，會破壞多型性：[example](example.linq)

## 多型
- 一個介面可以有多種不同的實現方式
- 父類別的參考可以指向子類別的物件
- 在執行時，會根據**物件的執行期類型**來決定應該呼叫的方法
