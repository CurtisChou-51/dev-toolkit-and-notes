# Full Replacement Pattern (EFCore)

- Full Replacement without Id

## 1. delete all + insert：
```csharp
_repository.DeleteRange(_repository.GetAll().Where(...));
_repository.AddRange(input.Select(item => new Target
{
    A = item.A,
    B = item.B
}));
```

## 2. update + insert + delete：
```csharp
var existsItem = _repository.GetAll().Where(...);

// update
foreach (var (inputItem, existsItem) in input.Zip(exists, (inputItem, existsItem) => (inputItem, existsItem)))
{
    existsItem.A = inputItem.A;
    existsItem.B = inputItem.B;
}

// insert
foreach (var item in input.Skip(exists.Count))
    _repository.Add(new Target
    {
        A = item.A,
        B = item.B
    });

// delete
foreach (var item in exists.Skip(input.Count))
    _repository.Delete(item);
```
