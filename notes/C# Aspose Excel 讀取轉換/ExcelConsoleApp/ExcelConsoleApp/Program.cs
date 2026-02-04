using Aspose.Cells;

namespace ExcelConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Workbook workBook = new Workbook("sample.csv");
            var worksheet = workBook.Worksheets[0];
            var converter = new AsposeSheetConverter<BankTransDto>(HeaderMapper, worksheet);

            // 自行檢查必要欄位
            CheckRequiredColumns(converter.YieldMappedColumnNames().ToArray());

            // 實際轉換並列印結果
            foreach (var data in converter.YieldDatas())
                Console.WriteLine($"{data.BankCode}, {data.AccountNumber}, {data.TransactionDate:yyyy-MM-dd}, {data.Amount}");
        }
    
        static void CheckRequiredColumns(string[] mappedCols)
        {
            string[] requiredCols =
            [
                nameof(BankTransDto.BankCode),
                nameof(BankTransDto.AccountNumber),
                nameof(BankTransDto.TransactionDate),
                nameof(BankTransDto.Amount)
            ];

            string[] missingCols = requiredCols.Except(mappedCols).ToArray();
            if (missingCols.Length != 0)
                Console.WriteLine($"缺少必要欄位 {string.Join(", ", missingCols)}");
        }

        static string? HeaderMapper(string? header)
        {
            // 自製標頭對應規則：去除控制字元並修剪空白
            string? normalizedHeader = header is null ? null : string.Concat(header.Where(c => !char.IsControl(c))).Trim();
            return normalizedHeader switch
            {
                "銀行代碼" => nameof(BankTransDto.BankCode),
                "帳號" => nameof(BankTransDto.AccountNumber),
                "帳戶名稱" => nameof(BankTransDto.AccountName),
                "交易日期" => nameof(BankTransDto.TransactionDate),
                "交易金額" => nameof(BankTransDto.Amount),
                "幣別" => nameof(BankTransDto.Currency),
                "交易說明" => nameof(BankTransDto.Description),
                "交易類型" => nameof(BankTransDto.TransactionType),
                _ => normalizedHeader
            };
        }
    }

    public class BankTransDto
    {
        /// <summary> 銀行代碼 </summary>
        public string? BankCode { get; set; }

        /// <summary> 帳號 </summary>
        public string? AccountNumber { get; set; }

        /// <summary> 帳戶名稱 </summary>
        public string? AccountName { get; set; }

        /// <summary> 交易日期 </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary> 交易金額 </summary>
        public decimal? Amount { get; set; }

        /// <summary> 幣別 </summary>
        public string? Currency { get; set; }

        /// <summary> 交易說明 </summary>
        public string? Description { get; set; }

        /// <summary> 交易類型（如存款、提款等） </summary>
        public string? TransactionType { get; set; }
    }
}
