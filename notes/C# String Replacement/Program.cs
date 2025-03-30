using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        // 測試資料
        var dictionary = GenerateDictionary(300);
        string template = GenerateTemplate(3000, dictionary.Keys);

        // test string.Replace
        var stopwatch = Stopwatch.StartNew();
        string resultLoop = ReplaceWithLoop(template, dictionary);
        stopwatch.Stop();
        Console.WriteLine($"string.Replace : {stopwatch.ElapsedMilliseconds} ms");

        // test Regex.Replace
        stopwatch.Restart();
        string resultRegex = ReplaceWithRegex(template, dictionary);
        stopwatch.Stop();
        Console.WriteLine($"Regex.Replace : {stopwatch.ElapsedMilliseconds} ms");

        Console.WriteLine($"same: {resultLoop == resultRegex}");
    }

    static string ReplaceWithLoop(string template, Dictionary<string, string> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            template = template.Replace(kvp.Key, kvp.Value);
        }
        return template;
    }

    static string ReplaceWithRegex(string template, Dictionary<string, string> dictionary)
    {
        string pattern = @"\[[A-Za-z]+\d+\]";
        return Regex.Replace(template, pattern, match =>
            dictionary.TryGetValue(match.Value, out var value) ? value : match.Value
        );
    }

    static Dictionary<string, string> GenerateDictionary(int count)
    {
        var random = new Random();
        var dict = new Dictionary<string, string>();
        for (int i = 0; i < count; i++)
        {
            string key = $"[{(char)('A' + random.Next(26))}{random.Next(1000000):D6}]";
            dict[key] = $"Value_{i}";
        }
        return dict;
    }

    static string GenerateTemplate(int markerCount, IEnumerable<string> keys)
    {
        var random = new Random();
        var template = new StringBuilder();
        var keyList = new List<string>(keys);
        for (int i = 0; i < markerCount; i++)
        {
            for (int j = 0; j < 10; j++)
                template.Append(Guid.NewGuid());
            template.Append(keyList[random.Next(keyList.Count)] + " ");
            for (int j = 0; j < 10; j++)
                template.Append(Guid.NewGuid());
        }
        return template.ToString();
    }
}