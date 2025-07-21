using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;

namespace DataAccess
{
    public class BaseDataAccess<TContext> where TContext : DbContext
    {

        protected readonly TContext _dbContext;

        /// <summary> 取得目前的連線 </summary>
        /// <remarks> 連線為透過 EF Core 取得，EF Core 會管理生命週期因此不用加 using 或是 Dispose </remarks>
        protected IDbConnection Connection => _dbContext.Database.GetDbConnection();

        protected IDbTransaction? CurrentTransaction => _dbContext.Database.CurrentTransaction?.GetDbTransaction();

        public BaseDataAccess(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary> EF Query </summary>
        public IQueryable<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        /// <summary> EF SaveChanges </summary>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary> EF BeginTransaction </summary>
        public IDbContextTransaction BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        /// <summary> Dapper 執行 SQL </summary>
        protected int DapperExecute(string sql, object parameters, int? commandTimeout = null)
            => Connection.Execute(sql, parameters, commandType: CommandType.Text, commandTimeout: commandTimeout, transaction: CurrentTransaction);

        /// <summary> Dapper 查詢 </summary>
        protected IEnumerable<T> DapperQuery<T>(string sql, object parameters, int? commandTimeout = null)
            => Connection.Query<T>(sql, parameters, commandType: CommandType.Text, commandTimeout: commandTimeout, transaction: CurrentTransaction);

        /// <summary> Dapper 查詢 FirstOrDefault </summary>
        protected T? DapperQueryFirstOrDefault<T>(string sql, object parameters, int? commandTimeout = null)
            => Connection.QueryFirstOrDefault<T>(sql, parameters, commandType: CommandType.Text, commandTimeout: commandTimeout, transaction: CurrentTransaction);
    }
}
