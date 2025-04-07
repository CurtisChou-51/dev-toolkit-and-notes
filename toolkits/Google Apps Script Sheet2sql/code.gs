/**
 * @OnlyCurrentDoc
 */

function printJson() {
  let sheets = SpreadsheetApp.getActiveSpreadsheet().getSheets();
   console.log(getJson(sheets));
}

function printSql() {
  let sheets = SpreadsheetApp.getActiveSpreadsheet().getSheets();
  console.log(getSql(sheets));
}

function doGet(e) {
  let type = e?.parameter?.type;
  let tmpl = HtmlService.createTemplateFromFile("Index.html");
  let sheets = SpreadsheetApp.getActiveSpreadsheet().getSheets();
  tmpl.sql = type === 'json' ? JSON.stringify(getJson(sheets), null, 4) : getSql(sheets);
  return tmpl.evaluate();
}

function getSql(sheets) {
  let sb = [];
  let json = getJson(sheets);
  for (let tableName in json) {
    sb.push("/* -------------------------------------------------------------------------------------- */");
    sb.push(`truncate table ${tableName};`);
    for (let row of json[tableName]) {
      const cols = Object.keys(row).join(', ');
      const values = Object.values(row).map(value => {
        if (typeof value === 'string') {
          return value === '' ? 'NULL' : `N'${value}'`;
        }
        return value;
      }).join(', ');
      sb.push(`insert into ${tableName} (${cols}) values (${values});`);
    }
  }
  return sb.join("\n");
}

function getJson(sheets) {
  let result = {};
  for (let sheet of sheets) {
    let tableName = sheet.getName();
    let sheetData = [];
    for (let json of yieldSheetToJson(sheet))
      sheetData.push(json);
    result[tableName] = sheetData;
  }
  return result;
}

function* yieldSheetToJson(sheet) {
  let sheetDatas = sheet.getDataRange().getValues();
  let headerAry = sheetDatas[0], table = sheetDatas.slice(1);
  for (let row of table) {
    let obj = {};
    for (let i = 0; i<headerAry.length; i++) {
      if (!headerAry[i])
        continue;
      obj[headerAry[i]] = row[i];
    }
    yield obj;
  }
}
