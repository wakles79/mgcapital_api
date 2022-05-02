using Dapper;
using MGCap.DataAccess.Abstract.Repository;
using MGCap.Domain.Models;
using MGCap.Domain.Options;
using MGCap.Domain.Utils;
using MGCap.Domain.ViewModels.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MGCap.DataAccess.Implementation.Repositories
{
    public class BaseDapperRepository : IBaseDapperRepository
    {
        public int Id { get; set; }

        protected readonly string _connString;

        public BaseDapperRepository(IOptions<MGCapDbOptions> mgcapDbOptions)
        {
            _connString = mgcapDbOptions.Value.ConnectionString;
        }

        #region READ

        public T QuerySingleOrDefault<T>(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            T result;

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.QuerySingleOrDefault<T>(query, pars, commandType: commandType);
            }

            return result;
        }

        public async Task<T> QuerySingleOrDefaultAsync<T>(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            T result;

            using (var conn = new SqlConnection(_connString))
            {
                result = await conn.QuerySingleOrDefaultAsync<T>(query, pars, commandType: commandType);
            }

            return result;
        }

        public IEnumerable<T> Query<T>(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            IEnumerable<T> result;

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Query<T>(query, pars, commandType: commandType);
            }

            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            IEnumerable<T> result;

            using (var conn = new SqlConnection(_connString))
            {
                result = await conn.QueryAsync<T>(query, pars, commandType: commandType);
            }

            return result;
        }

        public int Execute(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            int result;

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Execute(query, pars, commandType: commandType);
            }

            return result;
        }

        public async Task<int> ExecuteAsync(string query, DynamicParameters pars, CommandType? commandType = null)
        {
            int result;

            using (var conn = new SqlConnection(_connString))
            {
                result = await conn.ExecuteAsync(query, pars, commandType: commandType);
            }

            return result;
        }

        public DataSource<T> QueryPaged<T>(PagedQueryTemplate queryTemplate, DynamicParameters pars) where T : IGridViewModel
        {
            string query = QueryPaged(queryTemplate);

            DataSource<T> result = new DataSource<T>();

            using (var conn = new SqlConnection(_connString))
            {
                var response = conn.Query<T>(query, pars);
                if(response.Any())
                {
                    result.Count = response.FirstOrDefault().Count;
                    result.Payload = response;
                }
            }

            return result;
        }

        public async Task<DataSource<T>> QueryPagedAsync<T>(PagedQueryTemplate queryTemplate, DynamicParameters pars) where T : IGridViewModel
        {
            string query = QueryPaged(queryTemplate);

            DataSource<T> result = new DataSource<T>();

            using (var conn = new SqlConnection(_connString))
            {
                var response = await conn.QueryAsync<T>(query, pars);
                if (response.Any())
                {
                    result.Count = response.FirstOrDefault().Count;
                    result.Payload = response;
                }
            }

            return result;
        }

        public Tuple<T1, T2> MultiQuery<T1, T2>(string query1, string query2, DynamicParameters pars)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var result = conn.QueryMultiple(string.Format("{0} ; {1}", query1, query2), pars);
                return new Tuple<T1, T2>(result.ReadSingle<T1>(), result.ReadSingle<T2>());
            }
        }

        public async Task<Tuple<T1, T2>> MultiQueryAsync<T1, T2>(string query1, string query2, DynamicParameters pars)
        {
            using (var conn = new SqlConnection(_connString))
            {
                var result = await conn.QueryMultipleAsync(string.Format("{0} ; {1}", query1, query2), pars);

                T1 result1 = await result.ReadSingleAsync<T1>();
                T2 result2 = await result.ReadSingleAsync<T2>();

                return new Tuple<T1, T2>(result1, result2);
            }
        }

        #endregion

        #region Query Children

        public IEnumerable<TResult> QueryChildList<TResult, TChild>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id")
            where TResult : IEntityParentViewModel<TChild>
            where TChild : EntityViewModel
        {
            IEnumerable<TResult> result;
            var lookup = new Dictionary<int, TResult>();

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Query<TResult, TChild, TResult>(
                    query,
                    (t, tChild) =>
                    {
                        if (lookup.TryGetValue(t.ID, out TResult tEntry) == false)
                        {
                            tEntry = t;
                            tEntry.Children1 = new List<TChild>();
                            lookup.Add(tEntry.ID, tEntry);
                        }

                        if (tChild.ID > 0)
                            tEntry.Children1.Add(tChild);

                        return tEntry;
                    },
                    pars,
                    commandType: commandType,
                    splitOn: splitOnField)
                    .Distinct()
                    .ToList(); ;
            }

            return result;
        }

        public IEnumerable<TResult> QueryChildrenList<TResult, T1, T2>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id")
            where TResult : IEntityParentViewModel<T1, T2>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
        {
            IEnumerable<TResult> result;
            var lookup = new Dictionary<int, TResult>();

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Query<TResult, T1, T2, TResult>(
                    query,
                    (t, t1, t2) =>
                    {
                        if (lookup.TryGetValue(t.ID, out TResult tEntry) == false)
                        {
                            tEntry = t;
                            tEntry.Children1 = new List<T1>();
                            tEntry.Children2 = new List<T2>();
                            lookup.Add(tEntry.ID, tEntry);
                        }

                        if (t1.ID > 0 && tEntry.Children1.Any(c => c.ID.Equals(t1.ID)) == false)
                            tEntry.Children1.Add(t1);

                        if (t2.ID > 0 && tEntry.Children2.Any(c => c.ID.Equals(t2.ID)) == false)
                            tEntry.Children2.Add(t2);

                        return tEntry;
                    },
                    pars,
                    commandType: commandType,
                    splitOn: splitOnField)
                    .Distinct()
                    .ToList();
            }

            return result;
        }

        public IEnumerable<TResult> QueryChildrenList<TResult, T1, T2, T3>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
        {
            IEnumerable<TResult> result;
            var lookup = new Dictionary<int, TResult>();

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Query<TResult, T1, T2, T3, TResult>(
                    query,
                    (t, t1, t2, t3) =>
                    {
                        if (lookup.TryGetValue(t.ID, out TResult tEntry) == false)
                        {
                            tEntry = t;
                            tEntry.Children1 = new List<T1>();
                            tEntry.Children2 = new List<T2>();
                            tEntry.Children3 = new List<T3>();
                            lookup.Add(tEntry.ID, tEntry);
                        }

                        if (t1.ID > 0 && tEntry.Children1.Any(c => c.ID.Equals(t1.ID)) == false)
                            tEntry.Children1.Add(t1);

                        if (t2.ID > 0 && tEntry.Children2.Any(c => c.ID.Equals(t2.ID)) == false)
                            tEntry.Children2.Add(t2);

                        if (t3.ID > 0 && tEntry.Children3.Any(c => c.ID.Equals(t3.ID)) == false)
                            tEntry.Children3.Add(t3);

                        return tEntry;
                    },
                    pars,
                    commandType: commandType,
                    splitOn: splitOnField)
                    .Distinct()
                    .ToList();
            }

            return result;
        }

        public IEnumerable<TResult> QueryChildrenList<TResult, T1, T2, T3, T4>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3, T4>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
            where T4 : EntityViewModel
        {
            IEnumerable<TResult> result;
            var lookup = new Dictionary<int, TResult>();

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Query<TResult, T1, T2, T3, T4, TResult>(
                    query,
                    (t, t1, t2, t3, t4) =>
                    {
                        if (lookup.TryGetValue(t.ID, out TResult tEntry) == false)
                        {
                            tEntry = t;
                            tEntry.Children1 = new List<T1>();
                            tEntry.Children2 = new List<T2>();
                            tEntry.Children3 = new List<T3>();
                            tEntry.Children4 = new List<T4>();
                            lookup.Add(tEntry.ID, tEntry);
                        }

                        if (t1.ID > 0 && tEntry.Children1.Any(c => c.ID.Equals(t1.ID)) == false)
                            tEntry.Children1.Add(t1);

                        if (t2.ID > 0 && tEntry.Children2.Any(c => c.ID.Equals(t2.ID)) == false)
                            tEntry.Children2.Add(t2);

                        if (t3.ID > 0 && tEntry.Children3.Any(c => c.ID.Equals(t3.ID)) == false)
                            tEntry.Children3.Add(t3);

                        if (t4.ID > 0 && tEntry.Children4.Any(c => c.ID.Equals(t4.ID)) == false)
                            tEntry.Children4.Add(t4);

                        return tEntry;
                    },
                    pars,
                    commandType: commandType,
                    splitOn: splitOnField)
                    .Distinct()
                    .ToList();
            }

            return result;
        }

        public async Task<IEnumerable<TResult>> QueryChildListAsync<TResult, TChild>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id")
            where TResult : IEntityParentViewModel<TChild>
            where TChild : EntityViewModel
        {
            var result = await Task.Run(() => { return QueryChildList<TResult, TChild>(query, pars, commandType, splitOnField); });
            return result;
        }

        public async Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id")
            where TResult : IEntityParentViewModel<T1, T2>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
        {
            var result = await Task.Run(() => { return QueryChildrenList<TResult, T1, T2>(query, pars, commandType, splitOnField); });
            return result;
        }

        public async Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2, T3>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
        {
            var result = await Task.Run(() => { return QueryChildrenList<TResult, T1, T2, T3>(query, pars, commandType, splitOnField); });
            return result;
        }

        public async Task<IEnumerable<TResult>> QueryChildrenListAsync<TResult, T1, T2, T3, T4>(string query, DynamicParameters pars, CommandType? commandType = null, string splitOnField = "Id,Id,Id,Id")
            where TResult : IEntityParentViewModel<T1, T2, T3, T4>
            where T1 : EntityViewModel
            where T2 : EntityViewModel
            where T3 : EntityViewModel
            where T4 : EntityViewModel
        {
            var result = await Task.Run(() => { return QueryChildrenList<TResult, T1, T2, T3, T4>(query, pars, commandType, splitOnField); });
            return result;
        }

        #endregion

        #region Insert

        public int Insert<T>(T entity, string userEmail = "", int companyId = -1) where T: BaseEntity
        {
            int? newId;

            using (var conn = new SqlConnection(_connString))
            {
                if (string.IsNullOrEmpty(userEmail).Equals(false) && companyId > 0)
                {
                    entity.BeforeCreate(userEmail, companyId);
                }

                newId = conn.Insert<T>(entity);
            }

            return newId ?? -1;
        }

        public async Task<int> InsertAsync<T>(T entity, string userEmail = "", int companyId = -1) where T : BaseEntity
        {
            int? newId;

            using (var conn = new SqlConnection(_connString))
            {
                if(string.IsNullOrEmpty(userEmail).Equals(false) && companyId > 0)
                {
                    entity.BeforeCreate(userEmail, companyId);
                }

                newId = await conn.InsertAsync<T>(entity);
            }

            return newId ?? -1;
        }

        public int InsertRange<T>(IEnumerable<T> entities, string userEmail = "", int companyId = -1) where T : BaseEntity
        {
            if (entities.Any() == false)
                return 0;

            int result = 0;
            T first = entities.FirstOrDefault();
            string query = QueryInsert(first);

            if (string.IsNullOrEmpty(userEmail).Equals(false) && companyId > 0)
            {
                foreach (T entity in entities)
                {
                    entity.BeforeCreate(userEmail, companyId);
                }
            }

            using (var conn = new SqlConnection(_connString))
            {
                result = conn.Execute(query, entities);
            }

            return result;
        }

        public async Task<int> InsertRangeAsync<T>(IEnumerable<T> entities, string userEmail = "", int companyId = -1) where T : BaseEntity
        {
            if (entities.Any() == false)
                return 0;

            int result = 0;
            T first = entities.FirstOrDefault();

            string query = QueryInsert(first);

            if (string.IsNullOrEmpty(userEmail).Equals(false) && companyId > 0)
            {
                foreach (T entity in entities)
                {
                    entity.BeforeCreate(userEmail, companyId);
                }
            }

            using (var conn = new SqlConnection(_connString))
            {
                result = await conn.ExecuteAsync(query, entities);
            }

            return result;
        }

        #endregion

        #region Utils

        private string QueryPaged(PagedQueryTemplate queryTemplate)
        {
            return string.Format(@"
                {0}
                SELECT * FROM ( 
                    SELECT ROW_NUMBER() OVER(ORDER BY {1}) AS PagedNumber, COUNT(*) OVER() AS [Count], {2} 
                    FROM {3}
                    WHERE 1 = 1 {4} 
                ) AS q
                WHERE PagedNumber BETWEEN ( {5} * {6} + 1 ) AND ( ({5} + 1) * {6} ) ",
                string.IsNullOrEmpty(queryTemplate.PreQuery) ? string.Empty : queryTemplate.PreQuery,
                queryTemplate.Orders,
                queryTemplate.SelectFields,
                queryTemplate.FromTables,
                queryTemplate.Conditions,
                queryTemplate.PageNumber ?? 0,
                queryTemplate.RowsPerPage ?? 20);
        }

        private string QueryInsert<T>(T entity)
        {
            var attr = Attribute.GetCustomAttributes(typeof(T)).FirstOrDefault(a => a.GetType().Equals(typeof(TableAttribute)));
            string tableName = (attr as TableAttribute).Name;

            string query = string.Format("INSERT INTO {0} VALUES (", tableName);

            StringBuilder sb = new StringBuilder();
            foreach (var property in entity.GetType().GetProperties().OrderBy(p => p.Name))
            {
                if (property.CustomAttributes.Any(a => a.AttributeType.Name.Equals(typeof(IgnoreInsertAttribute).Name)))
                    continue;

                sb.Append(string.Format(" @{0}, ", property.Name));
            }

            sb = sb.Remove(sb.Length - 2, 2);

            return string.Format(" {0} {1} )", query, sb.ToString());
        }

        #endregion

        public SqlConnection GetConnection()
        {
            return new SqlConnection(this._connString);
        }
    }
}
