using Logiwa.Common.Configurations;
using System.Data.SqlClient;

namespace Logiwa.Common.Utils
{
    public static class StringBuilderUtils
    {
        public static string BuildConnectionString(ConfigurationOptions configurationOptions, bool isReadContext = false)
        {
            return new SqlConnectionStringBuilder
            {
                DataSource = configurationOptions.DbConnectionOptions?.Server,
                UserID = configurationOptions.DbConnectionOptions?.UserName,
                Password = configurationOptions.DbConnectionOptions?.Password,
                InitialCatalog = configurationOptions.DbConnectionOptions?.Database,
                ApplicationIntent = isReadContext ? ApplicationIntent.ReadOnly : ApplicationIntent.ReadWrite,
                MaxPoolSize = 1000,
                TrustServerCertificate = true
            }.ConnectionString;
        }
    }
}
