# JavaScript Refact Note - CascadeUnitDDLControl

- 對於 `CascadeUnitDDLControl createDDLHtml` 方法的重構筆記

## 重構描述

- 關注點分離：
  - `createDDLOptions` 產生選項資料
  - `createDDLHtml` 產生 HTML 字串

- 資料流調整：
  - Pipeline 式轉換使處理方向更清晰，原始資料 → 處理後的選項資料 → HTML
  - 避免了在處理過程中同時組合 HTML 的狀況
  
- 效能優化，資料預處理：
  - 使用 `Set` 更快速檢查選中狀態
  - 使用 `groupBy` 預先將子單位分組

## Before

```javaScript
class CascadeUnitDDLControl {
    // other ...

    createDDLHtml(lv2Units) {
        var html = "";
        if (this.defaultChildUnit != '') {
            var unitsStr = this.defaultChildUnit;
            var selectedUnits = unitsStr.split(',');
        }
        for (var i = 0; i < lv2Units.length; i++) {
            if (selectedUnits != undefined && selectedUnits.indexOf(lv2Units[i][this.valueField].toString()) !== -1) {
                html += '<option selected="selected" value=' + lv2Units[i][this.valueField] + '>' + lv2Units[i].name + '</option>';
            }
            else {
                html += '<option value=' + lv2Units[i][this.valueField] + '>' + lv2Units[i].name + '</option>';
            }

            $.each(this.units, (index, value) => {
                if (value.parentCode == lv2Units[i].Code) {
                    if (selectedUnits != undefined && selectedUnits.indexOf(value[this.valueField].toString()) !== -1) {
                        html += '<option selected="selected" value=' + value[this.valueField] + '>' + value.name + '</option>'
                    }
                    else {
                        html += '<option value=' + value[this.valueField] + '>' + value.name + '</option>'
                    }
                }
            });
        }
        return html;
    }
}
```

## After

```javaScript
class CascadeUnitDDLControl {
    // other ...

    createDDLHtml(lv2Units) {
        return this.createDDLOptions(lv2Units)
            .map(x => `<option ${x.selected ? 'selected' : ''} value='${x.value}'>${x.text}</option>`)
            .join('');
    }

    createDDLOptions(lv2Units) {
        let selectedUnits = new Set(this.defaultChildUnit ? this.defaultChildUnit.split(',') : []);
        let childUnitMap = Utils.groupBy(this.units, x => x.parentCode);  // groupBy polyfill
        let valueField = this.valueField;
        return lv2Units
            .flatMap(lv2Unit => {
                // 產生單位後接著產生其下層單位
                let childUnits = childUnitMap[lv2Unit.Code] ?? [];
                return [lv2Unit, ...childUnits];
            })
            .map(unit => ({
                text: unit.name,
                value: unit[valueField],
                selected: selectedUnits.has(unit[valueField].toString())
            }));
    }
}
```
