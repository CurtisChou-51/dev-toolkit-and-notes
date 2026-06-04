# C# 

## Property Pattern Matching
```csharp
if (service.Save(data) is { Success: false } result)
    return BadRequest(result.ErrorMessage);
```