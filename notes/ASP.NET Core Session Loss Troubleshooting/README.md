# ASP.NET Core Session Loss Troubleshooting

- 問題背景：ASP.NET Core 網站使用 Session 儲存使用者狀態，發現大部分狀況下使用正常，但特定操作後 Session 會遺失。在開發環境無法重現，僅在正式環境發生
- 網站沒有使用 Load Balancer，發生 Session 遺失前沒有重啟應用程式、也沒有登入超時，基本可以排除以上原因

## 已知導致 Session 遺失狀況

- 由特定第三方 OAuth 服務登入後
- 從其他外部網站連結進入該網站
- 使用 Chrome Developer Tool 的 "前往" 功能

![](01.png)

## 初步推測原因

- Session ID 透過 Cookie 傳遞，Session 遺失可能是 Cookie 沒有被瀏覽器帶回，觀察 Chrome Developer Tool 發現 Cookie 內的 Session ID 確實發生了改變
- 正式環境會套用 WAF，WAF 與 IIS 溝通可能走 http 協定而非 https，可能導致被設定為 "secure" 的 Cookie 無法傳送。這是一個可能原因，但應該不會只在特定操作後發生

## Response Set-Cookie 的怪異狀況

- 發現正式環境 Response Header 中的 Set-Cookie 出現了兩次 SameSite 的設定