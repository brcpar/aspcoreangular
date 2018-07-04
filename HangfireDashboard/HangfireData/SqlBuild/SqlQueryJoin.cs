using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace HangfireData.SqlBuild
{
    public interface ISqlQueryJoin
    {
        void Build(StringBuilder strBuilder);
    }

    public abstract class SqlQueryJoin<T,S> : ISqlQueryJoin
    {
        protected readonly SqlQueryTable<T> TableOne;
        protected readonly SqlQueryTable<S> TableTwo;
        protected string OnCondition;
        protected SqlQueryJoin(SqlQueryTable<T> tableOne, SqlQueryTable<S> tableTwo)
        {
            TableTwo = tableTwo;
            TableOne = tableOne;
        }

        public abstract void Build(StringBuilder strBuilder);

        public virtual SqlQueryTable<T> On<TProp,SProp>(Expression<Func<T,TProp>> expression1, Expression<Func<S, SProp>> expression2)
        {
            var t1Alias = !string.IsNullOrEmpty(TableOne.Alias) ? $"[{TableOne.Alias}]." : "";
            var t2Alias = !string.IsNullOrEmpty(TableTwo.Alias) ? $"[{TableTwo.Alias}]." : "";
            OnCondition = $"ON {t1Alias}[{SqlQueryColumn.GetColumnName(expression1)}] = {t2Alias}[{SqlQueryColumn.GetColumnName(expression2)}]";
            return TableOne;
        }

        public virtual SqlQueryTable<T> On(string colOne, string colTwo)
        {
            OnCondition = "ON " + colOne + " = " + colTwo;
            return TableOne;
        }

        public virtual SqlQueryTable<T> On(string onCondition)
        {
            OnCondition = $"ON {onCondition}";
            return TableOne;
        }
    }

    public class SqlQueryInnerJoin<T,S> : SqlQueryJoin<T,S>
    {
        public SqlQueryInnerJoin(SqlQueryTable<T> tableOne, SqlQueryTable<S> tableTwo): base(tableOne, tableTwo)
        {
        }

        public override void Build(StringBuilder strBuilder)
        {
            strBuilder.AppendLine($"INNER JOIN {TableTwo.TableFullyQualify()} {OnCondition}");
        }
    }

    public class SqlQueryLeftJoin<T, S> : SqlQueryJoin<T, S>
    {
        public SqlQueryLeftJoin(SqlQueryTable<T> tableOne, SqlQueryTable<S> tableTwo) : base(tableOne, tableTwo)
        {
        }

        public override void Build(StringBuilder strBuilder)
        {
            strBuilder.AppendLine($"LEFT JOIN {TableTwo.TableFullyQualify()} {OnCondition}");
        }
    }

    public class SqlQueryRightJoin<T, S> : SqlQueryJoin<T, S>
    {
        public SqlQueryRightJoin(SqlQueryTable<T> tableOne, SqlQueryTable<S> tableTwo) : base(tableOne, tableTwo)
        {
        }

        public override void Build(StringBuilder strBuilder)
        {
            strBuilder.AppendLine($"RIGHT JOIN {TableTwo.TableFullyQualify()} {OnCondition}");
        }
    }
}
