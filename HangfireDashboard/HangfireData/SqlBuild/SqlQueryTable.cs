using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HangfireData.SqlBuild
{
    public class SqlQueryTable<T> : IDisposable
    {
        private readonly string _tableName;
        private readonly string _schema;
        private string _alias;
        private readonly SqlQueryBuilder<T> _queryBuilder;
        private ISqlWhereClause _where;
        protected readonly List<ISqlWhereClause> _ands = new List<ISqlWhereClause>();
        protected readonly List<ISqlWhereClause> _ors = new List<ISqlWhereClause>();
        private readonly List<ISqlQueryJoin> _joins;
        private SqlQueryOrderBy<T> _orderBy;

        public string Alias { get { return _alias; } }

        public SqlQueryTable(SqlQueryBuilder<T> builder, string tableName, string schema = "dbo")
        {
            _queryBuilder = builder;
            _tableName = tableName;
            _schema = schema;
            _joins = new List<ISqlQueryJoin>();
        }

        public SqlQueryTable(SqlQueryBuilder<T> builder, string tableName, string schema, string tableAlias) : this(builder, tableName, schema)
        {
            _alias = tableAlias;
        }

        public SqlQueryTable<T> As(string alias)
        {
            _alias = alias;
            return this;
        }

        public SqlQueryWhere<T, TProp> Where<TProp>(string columnName)
        {
            var where = new SqlQueryWhere<T, TProp>(this, $"[{columnName}]", _queryBuilder.Parameters);
            _where = where;
            return where;
        }

        public SqlQueryWhere<T, TProp> Where<TProp>(string columnName, string tableAlias)
        {
            var where = new SqlQueryWhere<T, TProp>(this, $"[{tableAlias}].[{columnName}]", _queryBuilder.Parameters);
            _where = where;
            return where;
        }

        public SqlQueryWhere<T, TProp> Where<TProp>(Expression<Func<T,TProp>> expression)
        {
            var aliasClause = !string.IsNullOrEmpty(Alias) ? $"[{Alias}]." : "";
            var col = $"{aliasClause}[{SqlQueryColumn.GetColumnName(expression)}]";
            var where = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _where = where;
            return where;
        }

        public SqlQueryWhere<T, TProp> Where<TProp>(Expression<Func<T,TProp>> expression, string tableAlias)
        {
            var col = $"[{tableAlias}].[{SqlQueryColumn.GetColumnName(expression)}]";
            var where = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _where = where;
            return where;
        }

        public SqlQueryWhere<T, TProp> AndWhere<TProp>(Expression<Func<T, TProp>> expression)
        {
            var aliasClause = !string.IsNullOrEmpty(Alias) ? $"[{Alias}]." : "";
            var col = $"{aliasClause}[{SqlQueryColumn.GetColumnName(expression)}]";
            var andWhere = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _ands.Add(andWhere);
            return andWhere;
        }

        public SqlQueryWhere<T, TProp> AndWhere<TProp>(Expression<Func<T, TProp>> expression, string tableAlias)
        {
            var col = $"[{tableAlias}].[{SqlQueryColumn.GetColumnName(expression)}]";
            var andWhere = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _ands.Add(andWhere);
            return andWhere;
        }

        public SqlQueryWhere<T, TProp> AndWhere<TProp>(string columnName)
        {
            var andWhere = new SqlQueryWhere<T, TProp>(this, $"[{columnName}]", _queryBuilder.Parameters);
            _ands.Add(andWhere);
            return andWhere;
        }

        public SqlQueryWhere<T, TProp> AndWhere<TProp>(string columnName, string tableAlias)
        {
            var andWhere = new SqlQueryWhere<T, TProp>(this, $"[{tableAlias}].[{columnName}]", _queryBuilder.Parameters);
            _ands.Add(andWhere);
            return andWhere;
        }

        public SqlQueryWhere<T, TProp> OrWhere<TProp>(Expression<Func<T, TProp>> expression)
        {
            var aliasClause = !string.IsNullOrEmpty(Alias) ? $"[{Alias}]." : "";
            var col = $"{aliasClause}[{SqlQueryColumn.GetColumnName(expression)}]";
            var orWhere = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _ors.Add(orWhere);
            return orWhere;
        }

        public SqlQueryWhere<T, TProp> OrWhere<TProp>(Expression<Func<T, TProp>> expression, string tableAlias)
        {
            var col = $"[{tableAlias}].[{SqlQueryColumn.GetColumnName(expression)}]";
            var orWhere = new SqlQueryWhere<T, TProp>(this, col, _queryBuilder.Parameters);
            _ors.Add(orWhere);
            return orWhere;
        }

        public SqlQueryWhere<T, TProp> OrWhere<TProp>(string columnName)
        {
            var orWhere = new SqlQueryWhere<T, TProp>(this, $"[{columnName}]", _queryBuilder.Parameters);
            _ors.Add(orWhere);
            return orWhere;
        }

        public SqlQueryWhere<T, TProp> OrWhere<TProp>(string columnName, string tableAlias)
        {
            var orWhere = new SqlQueryWhere<T, TProp>(this, $"[{tableAlias}].[{columnName}]", _queryBuilder.Parameters);
            _ors.Add(orWhere);
            return orWhere;
        }

        public SqlQueryJoin<T,S> InnerJoin<S>(string tableName, string alias = null, string schema = "dbo")
        {
            var innerJoin = new SqlQueryInnerJoin<T,S>(this, new SqlQueryTable<S>(null, tableName , schema, alias));
            _joins.Add(innerJoin);
            return innerJoin;
        }

        public SqlQueryJoin<T, S> LeftJoin<S>(string tableName, string alias = null, string schema = "dbo")
        {
            var join = new SqlQueryLeftJoin<T, S>(this, new SqlQueryTable<S>(null, tableName, schema, alias));
            _joins.Add(join);
            return join;
        }

        public SqlQueryJoin<T, S> RightJoin<S>(string tableName, string alias = null, string schema = "dbo")
        {
            var join = new SqlQueryRightJoin<T, S>(this, new SqlQueryTable<S>(null, tableName, schema, alias));
            _joins.Add(join);
            return join;
        }

        public SqlQueryOrderBy<T> OrderByAsc<TProp>(Expression<Func<T,TProp>> expression, string tableAlias = null)
        {
            var orderByClause = new SqlQueryOrderBy<T>(this, SqlQueryColumn.GetColumnName(expression), tableAlias);
            _orderBy = orderByClause;
            return _orderBy;
        }
        public SqlQueryOrderBy<T> OrderByAsc(string column, string tableAlias = null)
        {
            var orderByClause = new SqlQueryOrderBy<T>(this, column, tableAlias);
            _orderBy = orderByClause;
            return _orderBy;
        }
        public SqlQueryOrderBy<T> OrderByDesc<TProp>(Expression<Func<T, TProp>> expression, string tableAlias = null)
        {
            var orderByClause = new SqlQueryOrderBy<T>(this, SqlQueryColumn.GetColumnName(expression), tableAlias, true);
            _orderBy = orderByClause;
            return _orderBy;
        }
        public SqlQueryOrderBy<T> OrderByDesc(string column, string tableAlias = null)
        {
            var orderByClause = new SqlQueryOrderBy<T>(this, column, tableAlias, true);
            _orderBy = orderByClause;
            return _orderBy;
        }

        public string TableFullyQualify()
        {
            if (!string.IsNullOrEmpty(_alias))
                return $"[{_schema}].[{_tableName}] (NOLOCK) {_alias}";
            return $"[{_schema}].[{_tableName}] (NOLOCK)";
        }

        private void Remainder(StringBuilder result)
        {
            foreach (var join in _joins)
            {
                join.Build(result);
            }

            if (_where != null)
            {
                result.Append("WHERE ");
                _where.Build(result);

                foreach (var andcondition in _ands)
                {
                    result.Append("AND ");
                    andcondition.Build(result);
                }

                foreach (var orcondition in _ors)
                {
                    result.Append("OR ");
                    orcondition.Build(result);
                }
            }
            if (_orderBy != null)
            {
                _orderBy.Build(result);
            }
        }

        public void Build(StringBuilder strBuilder)
        {
            if (!string.IsNullOrEmpty(_alias))
                strBuilder.AppendLine($"[{_schema}].[{_tableName}] (NOLOCK) {_alias}");
            else
                strBuilder.AppendLine($"[{_schema}].[{_tableName}] (NOLOCK)");
            Remainder(strBuilder);
        }

        public void Dispose()
        {
            _ands.Clear();
            _ors.Clear();
            _joins.Clear();
            _where?.Dispose();
        }
    }
}
