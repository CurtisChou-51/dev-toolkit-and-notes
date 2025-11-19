# ASP.NET Core Identity 驗證事件與 SecurityStamp 驗證行為

## 問題背景
- 在使用 ASP.NET Core Identity 的網站中，使用了設置 `options.Events.OnValidatePrincipal` 的方式檢查使用者 Claims schema 是否應該更新，例如：

```csharp
builder.Services.ConfigureApplicationCookie(options => {
    options.Events.OnValidatePrincipal = context =>
    {
        string? ver = context.Principal is null ? null : UserClaimsHelper.GeVer(context.Principal);
        if (ver != UserClaimsHelper.ClaimsVer)
            context.RejectPrincipal();
        return Task.CompletedTask;
    };
});
```

- 並且使用了 [客製化的 UserClaimsPrincipalFactory](https://github.com/CurtisChou-51/dev-toolkit-and-notes/tree/main/notes/ASP.NET%20Core%20%E8%AA%8D%E8%AD%89%E6%A9%9F%E5%88%B6#aspnet-core-identity) 來產生 Claims

- 這樣雖然達到驗證 Claims schema 的目的，但卻造成了一些問題：
  - **Identity 原生的 SecurityStampValidator 失效**
  - 客製化的 `UserClaimsPrincipalFactory` 不被呼叫

- 我透過了修改 `ValidationInterval`，並配置中斷點方便觀察。發現設置了 `OnValidatePrincipal` 之後，就不進入我客製化的 `UserClaimsPrincipalFactory`；但如果不設置 `OnValidatePrincipal`，則會依照 `ValidationInterval` 正常進入：
```csharp
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.FromSeconds(10);
});
```

---

## 問題原因

- `options.Events.OnValidatePrincipal` 是一個 **單一委派**，我使用這樣的方法手動設定它，會**直接覆蓋掉 Identity 預設掛入的事件**：
```csharp
// 這是 Identity 預設的設定，會被覆蓋掉
options.Events.OnValidatePrincipal = SecurityStampValidator.ValidatePrincipalAsync;
```

- 現在每次請求都會執行 `OnValidatePrincipal`，而預設的 `SecurityStampValidator` 不再執行，也不會進行使用者狀態驗證，我客製化的 `UserClaimsPrincipalFactory` 也不再被呼叫

- 這個狀況的機制是預設的 `SecurityStampValidator` 裡面會去呼叫 `SignInManager.CreateUserPrincipalAsync()` 來重建 Claims，參考官方 Source Code [SecurityStampValidator](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/SecurityStampValidator.cs)、[SignInManager](https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Core/src/SignInManager.cs)，因此如果覆蓋掉 `SecurityStampValidator`，就不會呼叫 `CreateUserPrincipalAsync()`，也就不會進入 `UserClaimsPrincipalFactory`

## 正確解法

- ❌ 不要直接設置 `options.Events.OnValidatePrincipal`
- ✅ 使用自訂的 `CustomSecurityStampValidator`，透過繼承 `SecurityStampValidator<TUser>`，在其中加入自訂驗證邏輯：

```csharp
public class CustomSecurityStampValidator : SecurityStampValidator<AspNetUser>
{
    private readonly UserManager<AspNetUser> _userManager;

    public CustomSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<AspNetUser> signInManager, ILoggerFactory loggerFactory, UserManager<AspNetUser> userManager) : base(options, signInManager, loggerFactory)
    {
        _userManager = userManager;
    }

    public override async Task ValidateAsync(CookieValidatePrincipalContext context)
    {
        // 執行原本的 SecurityStamp 驗證
        await base.ValidateAsync(context);
        var user = context.Principal;
        if (user == null)
            return;

        // 驗證版本號確保使用最新的 Claims Schema
        string? ver = UserClaimsHelper.GeVer(user);
        if (ver != UserClaimsHelper.ClaimsVer)
        {
            context.RejectPrincipal();
            return;
        }

        // 可加入其他自訂驗證邏輯
    }
}
```

並註冊：

```csharp
builder.Services.AddScoped<ISecurityStampValidator, CustomSecurityStampValidator>();
```

> [!NOTE]
> 使用這個方法還是需要呼叫 await base.ValidateAsync(context);  
> 才會執行原本的 SecurityStamp 驗證邏輯，確保使用者狀態正確  
> 並且原本如果有寫 OnValidatePrincipal 也要移除  
> 否則不會進入 CustomSecurityStampValidator

## 其他補充

- 如果有其他自訂驗證邏輯需要 ValidationInterval 控制，可以在 `CustomSecurityStampValidator` 中加入判斷，用 cookie 的 `IssuedUtc` 判斷是否到期：

```csharp
private bool NeedToCheck(CookieValidatePrincipalContext context)
{
    var issuedUtc = context.Properties?.IssuedUtc;
    var interval = Options.ValidationInterval;
    if (issuedUtc is null)
        return false;
    var elapsed = DateTimeOffset.UtcNow - issuedUtc.Value;
    return elapsed > interval;
}
```
