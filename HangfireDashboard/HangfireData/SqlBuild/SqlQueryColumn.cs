using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HangfireData.SqlBuild
{
    public class SqlQueryColumn<T> : IDisposable
    {
        private SqlQueryBuilder<T> _queryBuilder;
        private StringBuilder columnBuilder = new StringBuilder();
        private SqlQueryTable<T> _from;
        public SqlQueryColumn(SqlQueryBuilder<T> queryBuilder)
        {
            _queryBuilder = queryBuilder;
        }

        public SqlQueryColumn<T> Column<TProp>(Expression<Func<T, TProp>> expression, string tableAlias)
        {
            AddColumn(SqlQueryColumn.GetColumnName(expression), tableAlias);
            return this;
        }

        public SqlQueryColumn<T> Column<TProp>(Expression<Func<T, TProp>> expression)
        {
            AddColumn(SqlQueryColumn.GetColumnName(expression), null);
            return this;
        }

        public SqlQueryColumn<T> Column(string name)
        {
            AddColumn(name, null);
            return this;
        }

        public SqlQueryColumn<T> Column(string name, string tableAlias)
        {
            AddColumn(name, tableAlias);
            return this;
        }

        public SqlQueryTable<T> From(string tableName)
        {
            return _from = new SqlQueryTable<T>(_queryBuilder, tableName);
        }

        public SqlQueryTable<T> From(string tableName, string schema)
        {
            return _from = new SqlQueryTable<T>(_queryBuilder, tableName, schema);
        }

        private void AddColumn(string name, string tableAlias)
        {
            if (!string.IsNullOrEmpty(tableAlias))
                columnBuilder.Append($"[{tableAlias}].");
            columnBuilder.Append($"{(name != "*" ? "[" : "")}{name}{(name != "*" ? "]" : "")}, ");
        }

        public void Build(StringBuilder strBuilder)
        {
            var noExtraComma = columnBuilder.ToString();
            if (string.IsNullOrEmpty(noExtraComma))
                return;
            if (noExtraComma.Length >= 2)
                noExtraComma = noExtraComma.Substring(0, noExtraComma.Length - 2);
            strBuilder.AppendLine($"{noExtraComma}");
            strBuilder.Append("FROM ");
            _from?.Build(strBuilder);
        }

        public void Dispose()
        {
            columnBuilder.Clear();
            columnBuilder = null;
            _from?.Dispose();
        }
    }

    public class SqlQueryColumn
    {
        public static string GetColumnName<T, TProp>(Expression<Func<T, TProp>> columnExpression)
        {
            if (columnExpression.Body is MemberExpression memberExpression) return memberExpression.Member.Name;
            if (!(columnExpression.Body is ConstantExpression contantExpression))
                throw new ArgumentException("Invalid Column Expression");

            return Convert.ToString(contantExpression.Value);

        }
    }
}
