using Model.Public;

namespace [[ProjName]].Models.[[FuncName]]
{
    /// <summary> 主頁面Model </summary>
    public class [[FuncName]]ViewModel
    {
        /// <summary> 查詢條件 </summary>
        public [[FuncName]]ConditionModel ConditionModel { get; set; } = new [[FuncName]]ConditionModel();

        /// <summary> 查詢結果 </summary>
        public IEnumerable<[[FuncName]]ResultModel> ResultModel { get; set; } = new List<[[FuncName]]ResultModel>();
    }

    /// <summary> 查詢條件 </summary>
    public class [[FuncName]]ConditionModel
    {
        /// <summary> DisplayName </summary>
        public string? DisplayName { get; set; }

        /// <summary> 分頁 </summary>
        public PageModel PageModel { get; set; } = new PageModel();

        /// <summary> 排序 </summary>
        public SortModel SortModel { get; set; } = new SortModel();
    }

    /// <summary> 查詢結果 </summary>
    public class [[FuncName]]ResultModel : QueryPageResultModel
    {
        public string? DisplayName { get; set; }
        public long Id { get; set; }
    }
}
