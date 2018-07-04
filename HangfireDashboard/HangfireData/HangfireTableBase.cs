using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using HangfireData.Models;
using HangfireData.SqlBuild;

namespace HangfireData
{
    public abstract class DapperTableBase
    {
        protected readonly IDatabaseService _db;
        protected readonly IHangfireSack _sack;

        protected abstract string TableName { get; }
        protected virtual string Schema => "Hangfire";

        protected DapperTableBase(IHangfireSack sack)
        {
            _sack = sack;
            _db = _sack.DatabaseService;
        }

        public T GetById<T>(int id) where T : BaseEntity
        {
            // check query cache before hitting db.
            //if (_db.QueryCache.GetCache(id, out value))
            //    return value;
            var builder = new SqlQueryBuilder<T>();
            builder.SelectTopStar(1).From(TableName, Schema).Where(t => t.Id).EqualTo(id);

            var value = LoadSingle(builder);
            //if (value != null)
            //    _db.QueryCache.SetCacheItem(id, value);
            return value;
        }

        //public T GetByExternalId<T>(Guid id) where T : BaseEntity
        //{
        //    var builder = new SqlQueryBuilder<T>();
        //    builder.SelectTopStar(1).From(TableName, Schema).Where(t => t.ExternalGuid).EqualTo(id);
        //    return LoadSingle(builder);
        //}

        public T LoadSingle<T>(SqlQueryBuilder<T> builder) => _db.Connection.QueryFirstOrDefault<T>(builder.Build(), builder.Parameters);
        public IEnumerable<T> LoadCollection<T>(SqlQueryBuilder<T> builder) => _db.Connection.Query<T>(builder.Build(), builder.Parameters);
        public IEnumerable<T> LoadCollection<T>(string sql, object parms) => _db.Connection.Query<T>(sql, parms);

        public virtual int ExecuteNonQuery<T>(SqlQueryBuilder<T> builder) =>
            _db.Connection.Execute(builder.Build(), builder.Parameters);

        public virtual T ExecuteScalar<S, T>(SqlQueryBuilder<S> builder) => _db.Connection.ExecuteScalar<T>(builder.Build(), builder.Parameters);

        /// <summary>
        /// Temporary fix for bypassing bug in dapper that doesn't call SetValue for certain TypeHandler
        /// https://github.com/StackExchange/Dapper/issues/303
        /// </summary>
        /// <typeparam name="T">EntityBase</typeparam>
        /// <param name="item">Entity</param>
        private static void UpdateDateTimes<T>(T item)
        {
            var properties = item.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType != typeof(DateTime) && property.PropertyType != typeof(DateTime?)) continue;
                var dtValue = (DateTime?)property.GetValue(item, null);
                if (dtValue == null)
                    continue;
                switch (dtValue?.Kind)
                {
                    case DateTimeKind.Utc:
                        break;
                    case DateTimeKind.Local:
                        property.SetValue(item, dtValue?.ToUniversalTime(), null);
                        break;
                    case DateTimeKind.Unspecified:
                        property.SetValue(item, DateTime.SpecifyKind(dtValue.Value, DateTimeKind.Utc), null);
                        break;
                }
            }
        }
    }
}
