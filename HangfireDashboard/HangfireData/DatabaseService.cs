using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;

namespace HangfireData
{
    public interface IDatabaseService : IDisposable
    {
        SqlConnection Connection { get; }
        //IQueryCache QueryCache { get; }
    }

    public class DatabaseService : IDatabaseService
    {
        public SqlConnection Connection { get; }

        //public IQueryCache QueryCache { get; }

        public DatabaseService(IConnectionStringManager connectionStringManager)
        {
            //QueryCache = queryCache;

            Connection = new SqlConnection(connectionStringManager.AppDatabase);
            Connection.Open();
        }

        public void Dispose()
        {
            Connection?.Close();
            Connection?.Dispose();
        }

        public static void DatabaseMapperSetup()
        {
            DbString.IsAnsiDefault = true; // default to varchar vs nvarchar for inserts
            SqlMapper.AddTypeMap(typeof(string), DbType.AnsiString); // default to varchar vs nvarchar for query and parameterized variables
            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2); // map new datatype datetime2 to datetime
            SqlMapper.AddTypeHandler(new DateTimeHandler()); // add custom datetime handler to ensure UTC is being saved for any datetime
        }
    }

    public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            switch (value.Kind)
            {
                case DateTimeKind.Utc:
                    parameter.Value = value;
                    break;
                case DateTimeKind.Local:
                    parameter.Value = value.ToUniversalTime();
                    break;
                case DateTimeKind.Unspecified:
                    parameter.Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                    break;
            }
        }

        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
        }
    }
}
