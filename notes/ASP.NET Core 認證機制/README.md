# ASP.NET Core 認證機制


## 相關方法與 scheme 名稱

| 方法                                          | 參數用途                   | 說明                                                         |
| -------------------------------------------- | ------------------------- | ----------------------------------------------------------- |
| `AddAuthentication("XXX")`                   | **預設的認證方案名稱**       | 未指定 scheme 時，會使用這個名稱對應的方案（如 `[Authorize]`、`SignInAsync()`） |
| `AddCookie("YYY")`                           | **註冊 Cookie 認證方案名稱** | 註冊 Cookie 認證方案，並給定一個名稱                                 |
| `AddJwtBearer("ZZZ")`                        | **註冊 JWT 認證方案名稱**    | 註冊 JWT 認證方案，並給定一個名稱                                    |
| `SignInAsync("AAA")` / `SignOutAsync("AAA")` | **實際執行的認證方案名稱**    | 名稱需與已註冊的方案一致（例如 Cookie、JWT 方案）                      |

- 所有使用到的 scheme 名稱都必須出現在 AddCookie/AddJwtBearer (或是其他註冊方法) 註冊過才有效，否則執行時會出現錯誤如：`No sign-in authentication handler is registered for the scheme`


## Example

```csharp
services.AddAuthentication("CookieScheme") // 設定預設為 Cookie
    .AddCookie("CookieScheme")
    .AddJwtBearer("JwtScheme", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            // ...
        };
    });
```


## `[Authorize]` 屬性

- `[Authorize]` 也會使用 `AddAuthentication("這裡的值")` 所設定的 **預設認證方案**
- 依照此範例的情境，如果沒有指定 scheme 系統會嘗試使用 `"CookieScheme"` 來驗證，例如：

```csharp
[Authorize]
```

- 如果要讓 API 使用 JWT 驗證：

```csharp
[Authorize(AuthenticationSchemes = "JwtScheme")]
```


## Cookie 登入與登出

```csharp
await HttpContext.SignInAsync("CookieScheme", userPrincipal, new AuthenticationProperties
{
    IsPersistent = true,
    ExpiresUtc = DateTime.UtcNow.AddDays(14)
});

await HttpContext.SignOutAsync("CookieScheme");
```