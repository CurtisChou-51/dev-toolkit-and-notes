using [[ProjName]].DataAccess.Interface;
using [[ProjName]].Models.[[FuncName]];
using Dao;
using Dapper;
using Model.Public;

namespace [[ProjName]].DataAccess
{
    public class [[FuncName]]DataAccess : BaseDataAccess, I[[FuncName]]DataAccess
    {
        public [[FuncName]]DataAccess(IDataAccessContext context) : base(context) { }

        /// <summary> 查詢 </summary>
        public IEnumerable<[[FuncName]]ResultModel> Search([[FuncName]]ConditionModel vm)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Page", vm.PageModel.Page);
            parameters.Add("@PageSize", vm.PageModel.PageSize);
            parameters.Add("@DisplayName", vm.DisplayName);

            string sql = @$"
with tmp as (
    select Id, DisplayName
      from XXXTable t
     where ( @DisplayName is null or DisplayName like '%' + @DisplayName + '%' )
), 
tmpCnt as (
    select count(*) as TotalItemCount from tmp
)
select * from tmpCnt, tmp
 order by {GetOrderBy(vm.SortModel)}
offset(@Page - 1) * @PageSize rows fetch next @PageSize rows only
";
            return QueryPage<[[FuncName]]ResultModel>(sql, parameters, vm.PageModel);
        }

        private string GetOrderBy(SortModel sortModel)
        {
            string sortProp = sortModel.SortProp switch
            {
                "DisplayName" => sortModel.SortProp,
                _ => "Id"
            };
            string sortOrder = sortModel.SortOrder switch
            {
                "asc" or "desc" => sortModel.SortOrder,
                _ => "desc"
            };
            return $"{sortProp} {sortOrder}";
        }
    }
}
