using [[ProjName]].Models.[[FuncName]];

namespace [[ProjName]].Services.Interface
{
    public interface I[[FuncName]]Service
    {
        /// <summary> 查詢 </summary>
        IEnumerable<[[FuncName]]ResultModel> Search([[FuncName]]ConditionModel vm);
    }
}