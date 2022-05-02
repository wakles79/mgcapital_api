using Dapper;
using MGCap.Domain.Models;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Abstract.Repository
{
    public interface IBaseDapperRepository
    {
        #region READ

        T QuerySingleOrDefault<T>(string query, DynamicParameters pars, CommandType? commandType = null);

        Task<T> QuerySingleOrDefaultAsync<T>(string query, DynamicParameters pars, CommandType? commandType = null);

        IEnumerable<T> Query<T>(string query, DynamicParameters pars, CommandType? commandType = null);

        Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters pars, CommandType? commandType = null);

        int Execute(string query, DynamicParameters pars, CommandType? commandType = null);

        Task<int> ExecuteAsync(string query, DynamicParameters pars, CommandType? commandType = null);

        DataSource<T> QueryPaged<T>(PagedQueryTemplate queryTemplate, DynamicParameters pars) where T : IGridViewModel;

        Task<DataSource<T>> QueryPagedAsync<T>(PagedQueryTemplate queryTemplate, DynamicParameters pars) where T : IGridViewModel;

        Tuple<T1, T2> MultiQuery<T1, T2>(string query1, string query2, DynamicParameters pars);

        Task<Tuple<T1, T2>> MultiQueryAsync<T1, T2>(string query1, string query2, DynamicParameters pars);

        #endregion

        #region Query Children

        IEnumerable<TResult> QueryChildList<TResult, TChild>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id")
            where TResult : IEntityParentViewModel<TChild>
            where TChild : EntityViewModel;

        IEnumerable<TResult> QueryChildrenList<TResult, T1, T2>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id")
            where TResult : IEntityParentViewModel<T1, T2>
            where T1 : EntityViewModel
            where T2 : EntityViewModel;

        IEnumerable<TResult> QueryChildrenList<TResult, T1, T2, T3>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel;

        IEnumerable<TResult> QueryChildrenList<TResult, T1, T2, T3, T4>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3, T4>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
            where T4 : EntityViewModel;

        Task<IEnumerable<TResult>> QueryChildListAsync<TResult, TChild>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id")
            where TResult : IEntityParentViewModel<TChild>
            where TChild : EntityViewModel;

        Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id")
            where TResult : IEntityParentViewModel<T1, T2>
            where T1 : EntityViewModel
            where T2 : EntityViewModel;

        Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2, T3>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel;

        Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2, T3, T4>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3, T4>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
            where T4 : EntityViewModel;

        #endregion

        #region Insert

        int Insert<T>(T obj, string userEmail = "", int companyId = -1) where T : BaseEntity;

        Task<int> InsertAsync<T>(T obj, string userEmail = "", int companyId = -1) where T : BaseEntity;

        int InsertRange<T>(IEnumerable<T> entities, string userEmail = "", int companyId = -1) where T : BaseEntity;

        Task<int> InsertRangeAsync<T>(IEnumerable<T> entities, string userEmail = "", int companyId = -1) where T : BaseEntity;

        #endregion

        SqlConnection GetConnection();
    }
}
