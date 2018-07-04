using System;
using System.Linq.Expressions;
using System.Text;

namespace HangfireData.SqlBuild
{
    public class SqlQueryOrderBy<T>
    {
        private string _column;
        private string _direction;
        private string _alias;
        private StringBuilder _thenBy;
        private SqlQueryTable<T> _table;
        public SqlQueryOrderBy(SqlQueryTable<T> table, string column, string alias, bool descending = false)
        {
            _table = table;
            _column = column;
            _direction = descending ? "DESC" : "ASC";
            _alias = alias;
            _thenBy = new StringBuilder();
        }

        public void Build(StringBuilder builder)
        {
            if (!string.IsNullOrEmpty(_alias))
                builder.AppendLine($"ORDER BY [{_alias}].[{_column}] {_direction}{_thenBy}");
            else
                builder.AppendLine($"ORDER BY [{_column}] {_direction}{_thenBy}");            
        }

        public SqlQueryOrderBy<T> ThenByAsc(string column, string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                _thenBy.Append($", [{alias}].[{column}] ASC");
            else
                _thenBy.Append($", [{column}] ASC");
            return this;
        }

        public SqlQueryOrderBy<T> ThenByAsc<TProp>(Expression<Func<T,TProp>> expression)
        {
            if (!string.IsNullOrEmpty(_table.Alias))
                _thenBy.Append($", [{_table.Alias}].[{SqlQueryColumn.GetColumnName(expression)}] ASC");
            else
                _thenBy.Append($", [{SqlQueryColumn.GetColumnName(expression)}] ASC");
            return this;
        }

        public SqlQueryOrderBy<T> ThenByDesc(string column, string alias)
        {
            if (!string.IsNullOrEmpty(alias))
                _thenBy.Append($", [{alias}].[{column}] DESC");
            else
                _thenBy.Append($", [{column}] DESC");
            return this;
        }

        public SqlQueryOrderBy<T> ThenByDesc<TProp>(Expression<Func<T, TProp>> expression)
        {
            if (!string.IsNullOrEmpty(_table.Alias))
                _thenBy.Append($", [{_table.Alias}].[{SqlQueryColumn.GetColumnName(expression)}] DESC");
            else
                _thenBy.Append($", [{SqlQueryColumn.GetColumnName(expression)}] DESC");
            return this;
        }
    }
}
