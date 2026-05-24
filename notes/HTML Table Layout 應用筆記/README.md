# HTML Table Layout 應用筆記

- 以往在用 `<table>` 排版時，常會遇到「明明設了寬度卻被內容撐開」「欄寬比例怪怪的」「加了 `nowrap` 整張表變很寬」等問題，在這裡整理一些實務上的排版陷阱與解法
- 可開 [table-layout-demo.html](table-layout-demo.html) 並排觀察四個案例：原始破版 / `word-break: break-all` / `ellipsis` 截斷 / 綜合版。範例情境統一為「350px 紅框外層 + 三欄表（長 A 字串 / 中等 B 內容 / 日期 C）」

## 長字串撐開 table

- 預設情況下，瀏覽器會盡可能避免在「英數字串中間」斷行，當儲存格內有長 URL、Hash、無空白英文字串、長 ID 時，會把欄位甚至整張 table 撐到超出容器寬度
- 即便外層容器有設 `max-width` 或 `overflow: hidden`，table 本身仍可能溢出，造成水平捲軸或破版

```html
<div style="width: 350px; border: 2px dashed #ff4d4d; padding: 10px;">
  <table style="width: 100%; border-collapse: collapse;">
    <thead>
      <tr><th>A</th><th>B</th><th>C</th></tr>
    </thead>
    <tbody>
      <tr>
        <td>AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA</td>
        <td>內容BBBBBBBBBBBBB</td>
        <td>2025-01-01</td>
      </tr>
    </tbody>
  </table>
</div>
```

解法：對 cell 加上強制斷行屬性

```html
<td style="word-break: break-all;">...</td>
<!-- 或 -->
<td style="overflow-wrap: anywhere;">...</td>
```

## table-layout: fixed

### `min-width` / `max-width` 失效

- 切換到 `table-layout: fixed` 後，瀏覽器只認「第一列 cell 的 `width`」或 `<colgroup>` 的 `width`
- 後續所有 cell 上的 `width`、`min-width`、`max-width` 一律被忽略

```html
<table style="width: 100%; table-layout: fixed;">
  <thead>
    <tr>
      <th>A</th>
      <th style="max-width: 50px;">B</th>   <!-- ❌ 無效，B 欄仍為 1/3 寬 -->
      <th style="min-width: 200px;">C</th>  <!-- ❌ 無效，C 欄仍為 1/3 寬 -->
    </tr>
  </thead>
</table>
```

- 正確作法：把寬度設在第一列或 `<colgroup>` 的 `width`（注意是 `width`，不是 min/max）
- 若需要「彈性上限」（內容多就撐到 200px、少就縮）的效果，fixed 模式做不到，要回到 `table-layout: auto` 配合 `max-width`

### 搭配 colgroup 集中管理欄寬

- 比起在每個 `<td>` 上設 width，用 `<colgroup>` 集中管理更清楚
- fixed 模式下，瀏覽器只看 colgroup 的寬度，後續 cell 的 width 都會被忽略

```html
<table style="width: 100%; table-layout: fixed;">
  <colgroup>
    <col style="width: 80px;">   <!-- A 欄固定 -->
    <col>                        <!-- B 欄吃剩餘空間 -->
    <col style="width: 90px;">   <!-- C 欄貼合日期寬度 -->
  </colgroup>
  <thead>
    <tr><th>A</th><th>B</th><th>C</th></tr>
  </thead>
</table>
```

## white-space: nowrap

- 在 cell 上加 `white-space: nowrap` 可以避免「日期」「金額」「狀態」這類欄位被擠成多行

## 截斷顯示（ellipsis）

```html
<table style="width: 100%; table-layout: fixed;">
  <tr>
    <td style="overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
      AAAAAAAAAAAAAAAAAAAAAAA
    </td>
  </tr>
</table>
```

- 套用後 A 欄的長字串會被截斷為「AAAAAA…」，滑鼠 hover 也不會自動顯示完整內容（如需提示要另外加 `title` 屬性）
- 注意：`text-overflow: ellipsis` 在 `<td>` 上要生效，table 必須是 `table-layout: fixed`，否則 cell 會被內容撐開、`overflow: hidden` 沒有作用點
- 多行截斷需用 `display: -webkit-box` + `-webkit-line-clamp`，但在 td 內表現不一致，常見作法是改在 cell 內包一層 `<div>` 套用

```html
<td>
  <div style="display: -webkit-box;
              -webkit-line-clamp: 2;
              -webkit-box-orient: vertical;
              overflow: hidden;">
    很長的文字內容...
  </div>
</td>
```

## 綜合範例：line-clamp + nowrap 混用

- 實務上一張表常見的需求：
  - 「描述」「備註」等長文字欄 → 限制最多顯示 N 列，超出顯示「…」
  - 「日期」「金額」「狀態」等定型欄 → 整段不換行，欄寬剛好貼合
- 作法是長文字欄包一層 `<div>` 套 `-webkit-line-clamp`，日期欄用 `white-space: nowrap`

```html
<table style="width: 100%; table-layout: fixed;">
  <colgroup>
    <col>
    <col style="width: 70px;">
    <col style="width: 90px;">
  </colgroup>
  <thead>
    <tr><th>A</th><th>B</th><th>C</th></tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <div style="display:-webkit-box;
                    -webkit-line-clamp:3;
                    -webkit-box-orient:vertical;
                    overflow:hidden;
                    word-break:break-all;">
          AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA...
        </div>
      </td>
      <td style="word-break:break-all;">內容BBBBBBBBBBBBB</td>
      <td style="white-space:nowrap;">2025-01-01</td>
    </tr>
  </tbody>
</table>
```

- `-webkit-line-clamp` 一定要寫在 `<div>` 上，不能直接寫在 `<td>`
- `word-break: break-all` 確保 div 內的無空白字串能斷行
- 日期欄的 `nowrap` 在 fixed + colgroup 已給定寬度的情況下其實可省略，但如果不配置固定寬度時會使用到

## 對照表

| 問題                          | 解法                                                                  |
| ----------------------------- | -------------------------------------------------------------------- |
| 長 URL / Hash 撐開 table       | `word-break: break-all` 或 `overflow-wrap: anywhere`                  |
| 設了 `width` 卻無效            | 考慮是否套用 `table-layout: fixed`                                              |
| 數字 / 日期被擠成多行           | `white-space: nowrap` 或明確 `width`                                  |
| 內容過長想要省略號              | `overflow: hidden` + `text-overflow: ellipsis` + `white-space: nowrap`（需 fixed） |
| 集中管理欄寬                   | `<colgroup>` + `<col>`，比逐欄設 width 清楚                            |
| 大表格 render 卡頓             | `table-layout: fixed` 不需等內容掃完即可 layout                         |
| fixed 模式下 `min-width` / `max-width` 沒效 | fixed 模式不認 min/max，改用第一列或 `<colgroup>` 的 `width` |
| 長文字限制 N 列截斷               | 包一層 `<div>` 套 `-webkit-line-clamp: N` + `-webkit-box-orient: vertical` + `overflow: hidden` |
