# C# 方法修飾詞
- 複習 C# abstract、virtual、override、new 方法修飾詞與多型

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