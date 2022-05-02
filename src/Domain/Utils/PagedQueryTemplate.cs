using System;
using System.Collections.Generic;
using System.Text;

namespace MGCap.Domain.Utils
{
    public class PagedQueryTemplate
    {
        /// <summary>
        /// Use this property for declaring some previsous query. Such as getting EmployeeId by email and company.
        /// This previous query must be defined as it is.
        /// </summary>
        public string PreQuery { get; set; }

        /// <summary>
        /// Do not include the `SELECT` statement, just fields with optional aliases.
        /// Ex: TableName1.PropertyName1 AS MyProperty1, TableName2.PropertyName2 AS MyProperty2, 
        /// </summary>
        public string SelectFields { get; set; }

       /// <summary>
       /// Do not include the `FROM` statement, just table name with optional aliases and joins.
       /// Ex: MyTable AS TableName1 LEFT OUTER JOIN MyTable2 AS TableName2 ON TableName1.PropertyName1 = TableName2.PropertyName2
       /// </summary>
        public string FromTables { get; set; }

        /// <summary>
        /// Do not include the `WHERE` statement, just fields and comparison/logical operators/parameters.
        /// Ex: TableName1.PropertyName1 <> 0 AND TableName2.PropertyName2 = @param2
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// MANDATORY!! Use at least one order criteria. Do not include the `ORDER BY` statement, just fields and optional order criteria.
        /// Ex: TableName1.PropertyName1 ASC, TableName2.PropertyName2 DESC
        /// </summary>
        public string Orders { get; set; }

        /// <summary>
        /// Default value: 0
        /// </summary>
        public int? PageNumber { get; set; }

        /// <summary>
        /// Default value: 20
        /// </summary>
        public int? RowsPerPage { get; set; }
    }
}
