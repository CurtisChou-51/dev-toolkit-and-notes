<Query Kind="Program" />

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

void Main()
{
    string table = "Person";
    InsertSql<PersonDto>(table).Dump();
    UpdateSql<PersonDto>(table, "Id").Dump();
}

public static string InsertSql<T>(string tableName)
{
    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    var columnNames = string.Join(", ", properties.Select(p => $"{p.Name}"));
    var paramNames = string.Join(", ", properties.Select(p => $"@{p.Name}"));

    return $@"insert into {tableName}
  ({columnNames}) 
values
  ({paramNames});";
  
}

public static string UpdateSql<T>(string tableName, string keyColumn)
{
    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
    var setClauses = properties
        .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase))
        .Select((p, idx) => $"{(idx > 0 ? $"       {p.Name}" : p.Name)} = @{p.Name}");

    return $@"update {tableName} 
   set {string.Join(",\n", setClauses)}
 where {keyColumn} = @{keyColumn};";
  
}