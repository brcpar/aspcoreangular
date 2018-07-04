using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HangfireData.SqlBuild
{
    public class SqlQueryBuilder<T> : IDisposable
    {
        private SqlQueryColumn<T> column;
        private int top = 0;
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public SqlQueryColumn<T> Select()
        {
            return column = new SqlQueryColumn<T>(this);
        }
        public SqlQueryColumn<T> SelectTop(int topCount)
        {
            top = topCount;
            return Select();
        }
        public SqlQueryColumn<T> SelectStar()
        {
            column = new SqlQueryColumn<T>(this);
            column.Column("*");
            return column;
        }
        public SqlQueryColumn<T> SelectTopStar(int topCount)
        {
            top = topCount;
            column = new SqlQueryColumn<T>(this);
            column.Column("*");
            return column;
        }

        public string Build()
        {
            var strBuilder = new StringBuilder();
            if (top > 0)
                strBuilder.Append($"SELECT TOP ({top}) ");
            else
                strBuilder.Append("SELECT ");
            column.Build(strBuilder);
            return strBuilder.ToString();
        }

        public void Dispose()
        {
            column?.Dispose();
            column = null;
            Parameters.Clear();
            Parameters = null;
        }
    }
}
