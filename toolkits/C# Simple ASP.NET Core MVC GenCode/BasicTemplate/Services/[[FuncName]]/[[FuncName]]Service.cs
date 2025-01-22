using [[ProjName]].DataAccess.Interface;
using [[ProjName]].Models.[[FuncName]];
using [[ProjName]].Services.Interface;

namespace [[ProjName]].Services
{
    public class [[FuncName]]Service : I[[FuncName]]Service
    {
        private readonly I[[FuncName]]DataAccess _dbAccess;

        public [[FuncName]]Service(I[[FuncName]]DataAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        /// <summary> 查詢 </summary>
        public IEnumerable<[[FuncName]]ResultModel> Search([[FuncName]]ConditionModel vm)
        {
            return _dbAccess.Search(vm);
        }
    }
}
