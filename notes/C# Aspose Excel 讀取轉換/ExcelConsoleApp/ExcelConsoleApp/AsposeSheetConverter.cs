using Aspose.Cells;
using System.Reflection;

namespace ExcelConsoleApp
{
    public class AsposeSheetConverter<T> where T : new()
    {
        private readonly Func<string?, string?> _headerMapper;
        private readonly Worksheet _worksheet;
        private readonly Dictionary<int, PropertyInfo> _propMap;

        public AsposeSheetConverter(Func<string?, string?> headerMapper, Worksheet worksheet)
        {
            _headerMapper = headerMapper;
            _worksheet = worksheet;
            _propMap = BuildPropMap(_worksheet);
        }

        public IEnumerable<string> YieldMappedColumnNames()
        {
            foreach (var kv in _propMap)
                yield return kv.Value.Name;
        }

        public IEnumerable<T> YieldDatas()
        {
            for (int rowIdx = 1; rowIdx <= _worksheet.Cells.MaxDataRow; rowIdx++)
            {
                Row? row = _worksheet.Cells.CheckRow(rowIdx);
                if (row is null || row.IsBlank)
                    continue;
                T dto = ConvertRowToDto(row);
                yield return dto;
            }
        }

        private Dictionary<int, PropertyInfo> BuildPropMap(Worksheet worksheet)
        {
            var propMap = new Dictionary<int, PropertyInfo>();
            Row? row = _worksheet.Cells.CheckRow(0);
            if (row is null || row.IsBlank)
                return propMap;

            for (int colIdx = 0; colIdx <= worksheet.Cells.MaxDataColumn; colIdx++)
            {
                // Aspse 的索引器 (Cells[rowIdx, colIdx]) 在 Cell 不存在時會建立一個新的，因此這邊改用 GetCellOrNull 來避免不必要的物件建立
                Cell? cell = row.GetCellOrNull(colIdx);
                if (cell?.Value?.ToString() is not string headerText)
                    continue;
                string? propName = _headerMapper(headerText);
                if (string.IsNullOrEmpty(propName))
                    continue;
                PropertyInfo? prop = typeof(T).GetProperty(propName);
                if (prop != null && prop.CanWrite)
                    propMap[colIdx] = prop;
            }
            return propMap;
        }

        private T ConvertRowToDto(Row row)
        {
            T dto = new();
            foreach (var (colIdx, prop) in _propMap)
            {
                // Aspse 的索引器 (Cells[rowIdx, colIdx]) 在 Cell 不存在時會建立一個新的，因此這邊改用 GetCellOrNull 來避免不必要的物件建立
                Cell? cell = row.GetCellOrNull(colIdx);
                if (cell is null || cell.Value is null)
                    continue;

                object cellValue = cell.Value;
                Type convertTo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                if (convertTo.IsAssignableFrom(cellValue.GetType()))
                {
                    prop.SetValue(dto, cellValue);
                }
                else
                {
                    try
                    {
                        prop.SetValue(dto, Convert.ChangeType(cellValue, convertTo));
                    }
                    catch
                    {
                        prop.SetValue(dto, null);
                    }
                }
            }
            return dto;
        }
    }
}
