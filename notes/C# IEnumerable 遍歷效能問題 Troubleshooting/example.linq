<Query Kind="Program" />

void Main()
{
    DataTable dt = GetData();
    var searchResult = DataTableToModels<XXXResultModel>(dt);
    
    Render1(searchResult);
    "Render1 Complete".Dump();
    
    Render2(searchResult);
    "Render2 Complete".Dump();
}

void Render1(IEnumerable<XXXResultModel> searchResult)
{
    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < searchResult.Count(); i++)
    {
        sb.Append($@"
A:{searchResult.ElementAt(i).A}
B:{searchResult.ElementAt(i).B}
C:{searchResult.ElementAt(i).C}
D:{searchResult.ElementAt(i).D}
E:{searchResult.ElementAt(i).E}");
    }
}

void Render2(IEnumerable<XXXResultModel> searchResult)
{
    StringBuilder sb = new StringBuilder();
    foreach (var item in searchResult)
    {
        sb.Append($@"
A:{item.A}
B:{item.B}
C:{item.C}
D:{item.D}
E:{item.E}");
    }
}

DataTable GetData()
{
    DataTable dt = new DataTable();
    dt.Columns.Add("A");
    dt.Columns.Add("B");
    dt.Columns.Add("C");
    dt.Columns.Add("D");
    dt.Columns.Add("E");
    for (int i=0; i<300; i++)
        dt.Rows.Add(i, $"B-{i}", $"C-{i}", $"D-{i}", $"E-{i}");
    return dt;
}

class XXXResultModel
{
    public int A { get; set; }
    public string B { get; set; }
    public string C { get; set; }
    public string D { get; set; }
    public string E { get; set; }
}

static IEnumerable<T> DataTableToModels<T>(DataTable dt) where T : new()
{
    PropertyInfo[] properties = typeof(T).GetProperties();
    foreach (DataRow row in dt.Rows)
    {
        T item = new T();
        foreach (PropertyInfo property in properties)
        {
            if (!dt.Columns.Contains(property.Name))
                continue;
            object value = row[property.Name];
            if (value != DBNull.Value)
            {
                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                property.SetValue(item, Convert.ChangeType(value, convertTo));
            }
        }
        yield return item;
    }
}