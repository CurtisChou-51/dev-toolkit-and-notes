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