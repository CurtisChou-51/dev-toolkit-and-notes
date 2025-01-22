using [[ProjName]].Models.[[FuncName]];

namespace [[ProjName]].DataAccess.Interface
{
    public interface I[[FuncName]]DataAccess
    {
        IEnumerable<[[FuncName]]ResultModel> Search([[FuncName]]ConditionModel vm);
    }
}