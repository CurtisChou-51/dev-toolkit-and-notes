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