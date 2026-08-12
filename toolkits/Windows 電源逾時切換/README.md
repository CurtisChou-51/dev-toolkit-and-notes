# Windows 電源逾時切換

- 用途：以 `powercfg` 一鍵切換「關閉螢幕」與「進入睡眠」的閒置逾時，避免電腦在閒置導致 VPN 斷線、console 程式被凍結

## 動機

- 資安規定要求電腦必須設定閒置逾時，所以平時維持一般電源 5 分鐘、電池 3 分鐘的設定
- 但執行耗時較長的任務時（批次程式、長時間查詢、需要保持 VPN 連線），機器一進入睡眠就會中斷，如果人不在機器前須暫時關閉逾時，因此做成腳本快速設置與還原

## 主要功能

- `power-timeout-off.bat`：四項逾時全部設為 `0`（永不）

| 電源 | 關閉螢幕 | 進入睡眠 |
| --- | --- | --- |
| 一般電源 | 永不 | 永不 |
| 電池使用中 | 永不 | 永不 |

- `power-timeout-restore.bat`：還原為原本設定

| 電源 | 關閉螢幕 | 進入睡眠 |
| --- | --- | --- |
| 一般電源 | 5 分鐘 | 5 分鐘 |
| 電池使用中 | 3 分鐘 | 3 分鐘 |

- `check-ui.bat`：叫出控制台的「編輯計劃設定」頁面，切換完可以立刻確認生效

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

## 確認目前設定值

```bash
powercfg /query SCHEME_CURRENT SUB_VIDEO VIDEOIDLE
```

```bash
powercfg /query SCHEME_CURRENT SUB_SLEEP STANDBYIDLE
```

- 輸出的「目前的 AC/DC 電源設定索引」是**十六進位的秒數**，例如 `0x0000012c` = 300 秒 = 5 分鐘

- 想用 GUI 確認就執行 `check-ui.bat`，內容等同於：

```bash
control /name Microsoft.PowerOptions /page pagePlanSettings
```

