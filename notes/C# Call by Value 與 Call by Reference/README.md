# C# Call by Value 與 Call by Reference

- 複習 C# Call by Value 與 Call by Reference

## Call by Value 定義
- 當函數被呼叫時，實際參數的值會被複製一份然後傳遞給函數。函數內部對參數的任何修改都只會影響這個複製品，不會影響到原始的變數

## Call by Reference 定義
- 當函數被呼叫時，傳遞的是變數的記憶體位址（或參考），而不是值的複製品。函數內部直接操作原始變數的記憶體位置，因此對參數的修改會直接影響到原始變數

## C# 中的 Call by Value 與 Call by Reference
- C# 預設都是使用 Call by Value，參數加上 `ref` 或 `out` 關鍵字才會是 Call by Reference

- 如果傳入的是 reference type（如 class）也同樣是 Call by Value，此時傳入的參數是**物件的參考的複製品**

```csharp
void Change(Person p)
{
    p = new Person { Name = "Mary" };
}

var person = new Person { Name = "John" };
Change(person);
Console.WriteLine(person.Name);   // John
```

```csharp
void Change(ref Person p)
{
    p = new Person { Name = "Mary" };
}

var person = new Person { Name = "John" };
Change(ref person);
Console.WriteLine(person.Name);   // Mary
```