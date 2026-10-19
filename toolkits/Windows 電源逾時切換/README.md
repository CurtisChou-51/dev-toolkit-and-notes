# Windows 電源逾時切換

- 用途：以 `powercfg` 一鍵切換「關閉螢幕」與「進入睡眠」的閒置逾時，並一併切換螢幕保護裝置與閒置鎖定，避免電腦在閒置導致 VPN 斷線、console 程式被凍結

## 動機

- 資安規定要求電腦必須設定閒置逾時，所以平時維持一般電源 5 分鐘、電池 3 分鐘的設定
- 但執行耗時較長的任務時（批次程式、長時間查詢、需要保持 VPN 連線），機器一進入睡眠就會中斷，如果人不在機器前須暫時關閉逾時，因此做成腳本快速設置與還原

## 主要功能

- `power-timeout-off.bat`：四項逾時全部設為 `0`（永不），並停用螢幕保護裝置與閒置鎖定

| 電源 | 關閉螢幕 | 進入睡眠 |
| --- | --- | --- |
| 一般電源 | 永不 | 永不 |
| 電池使用中 | 永不 | 永不 |

| 螢幕保護裝置設定畫面項目 | 切換後 |
| --- | --- |
| 螢幕保護裝置 | 停用，下拉顯示「(無)」 |
| 等候 | 不變更 |
| 繼續執行後，顯示登入畫面 | 取消勾選 |

- `power-timeout-restore.bat`：還原為原本設定

| 電源 | 關閉螢幕 | 進入睡眠 |
| --- | --- | --- |
| 一般電源 | 5 分鐘 | 5 分鐘 |
| 電池使用中 | 3 分鐘 | 3 分鐘 |

| 螢幕保護裝置設定畫面項目 | 切換後 |
| --- | --- |
| 螢幕保護裝置 | 啟用，下拉回到原本選的程式 |
| 等候 | 5 分鐘 |
| 繼續執行後，顯示登入畫面 | 勾選 |

- `check-ui.bat`：同時叫出控制台的「編輯計劃設定」與「螢幕保護裝置設定」兩個頁面，切換完可以立刻確認生效

- 以上兩支切換用的 bat 都可直接雙擊執行，執行完會停在畫面等按鍵；從終端機呼叫時則不會 pause（以 `%cmdcmdline%` 判斷啟動方式）

## 對應的 powercfg 參數

| 設定畫面項目 | 參數 | 單位 |
| --- | --- | --- |
| 一般電源 → 關閉螢幕 | `monitor-timeout-ac` | 分鐘，`0` = 永不 |
| 電池使用中 → 關閉螢幕 | `monitor-timeout-dc` | 分鐘，`0` = 永不 |
| 一般電源 → 進入睡眠 | `standby-timeout-ac` | 分鐘，`0` = 永不 |
| 電池使用中 → 進入睡眠 | `standby-timeout-dc` | 分鐘，`0` = 永不 |

- 只影響**目前作用中的電源方案**，`powercfg /list` 可查看所有方案
- 一般使用者帳號通常即可執行；失敗時腳本會提示改用系統管理員身分

## 對應的螢幕保護裝置登錄檔值

- 位於 `HKCU\Control Panel\Desktop`，型別皆為 `REG_SZ`，只影響**目前登入的使用者**

| 設定畫面項目 | 登錄檔值 | `off` 寫入 | `restore` 寫入 |
| --- | --- | --- | --- |
| 螢幕保護裝置（啟用與否） | `ScreenSaveActive` | `0` | `1` |
| 等候 | `ScreenSaveTimeOut` | 不變更 | `300`（秒） |
| 繼續執行後，顯示登入畫面 | `ScreenSaverIsSecure` | `0` | `1` |
| 下拉選單選的螢幕保護裝置程式 | `SCRNSAVE.EXE` | 不變更 | 不變更 |

- 只改登錄檔要**重新登入**才會生效，所以兩支腳本都另外以 PowerShell 呼叫 `user32.dll` 的 `SystemParametersInfo`，立即套用到目前工作階段

| 常數 | `uiAction` | 對應的登錄檔值 |
| --- | --- | --- |
| `SPI_SETSCREENSAVETIMEOUT` | `15` | `ScreenSaveTimeOut` |
| `SPI_SETSCREENSAVEACTIVE` | `17` | `ScreenSaveActive` |
| `SPI_SETSCREENSAVESECURE` | `119` | `ScreenSaverIsSecure` |

- 第四個參數 `fWinIni` 傳 `3`（`SPIF_UPDATEINIFILE` + `SPIF_SENDCHANGE`），同時寫回設定檔並廣播變更通知
- `restore` 寫回的是**寫死的值**（5 分鐘、需登入），不是執行 `off` 之前的原值；若原本設定與這組不同，還原後會被改掉

## 確認目前設定值

```bash
powercfg /query SCHEME_CURRENT SUB_VIDEO VIDEOIDLE
```

```bash
powercfg /query SCHEME_CURRENT SUB_SLEEP STANDBYIDLE
```

- 輸出的「目前的 AC/DC 電源設定索引」是**十六進位的秒數**，例如 `0x0000012c` = 300 秒 = 5 分鐘

- 螢幕保護裝置的幾個值一次查：

```bash
powershell -NoProfile -Command "Get-ItemProperty 'HKCU:\Control Panel\Desktop' | Select-Object ScreenSaveActive, ScreenSaveTimeOut, ScreenSaverIsSecure, 'SCRNSAVE.EXE'"
```

- 想用 GUI 確認就執行 `check-ui.bat`，內容等同於依序開啟這兩個頁面：

```bash
control /name Microsoft.PowerOptions /page pagePlanSettings
```

```bash
control desk.cpl,,@screensaver
```

