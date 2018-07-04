using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HangfireData.SqlBuild
{
    public interface ISqlWhereClause : IDisposable
    {
        void Build(StringBuilder stringBuilder);
    }

    public class SqlQueryWhere<T, TProp> : ISqlWhereClause
    {
        private string _column;
        private readonly SqlQueryTable<T> _table;
        private StringBuilder _where;
        private readonly Dictionary<string, object> _parameters;

        public SqlQueryWhere(SqlQueryTable<T> table, string columnName, Dictionary<string, object> parameters)
        {
            _column = columnName;
            _table = table;
            _where = new StringBuilder();
            _parameters = parameters;
        }

        public SqlQueryTable<T> EqualTo(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} = @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> NotEqualTo(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} <> @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> In(IEnumerable<TProp> value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} IN @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> NotIn(IEnumerable<TProp> value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} NOT IN @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> GreaterThan(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} > @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> LessThan(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} < @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> LessThanOrEqualTo(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} <= @{parmName}");
            return _table;
        }

        public SqlQueryTable<T> GreaterThanOrEqualTo(TProp value)
        {
            var parmName = OnlyColumn();
            if (_parameters.ContainsKey(parmName))
                parmName += _parameters.Count.ToString();
            _parameters.Add(parmName, value);
            _where.Append($"{_column} >= @{parmName}");
            return _table;
        }

        private string OnlyColumn()
        {
            var cs = _column.Split('.');
            return cs.Last().Replace("[", "").Replace("]", "");
        }

        public void Build(StringBuilder strBuilder)
        {
            strBuilder.Append(_where);
            strBuilder.AppendLine();
        }

        public void Dispose()
        {
            _where.Clear();
            _where = null;
        }
    }
}
