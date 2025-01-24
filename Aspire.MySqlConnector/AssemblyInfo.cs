using Aspire.MySqlConnector;
using Aspire;

[assembly: ConfigurationSchema("Aspire:MySqlConnector", typeof(MySqlConnectorSettings))]

[assembly: LoggingCategories(
    "MySqlConnector",
    "MySqlConnector.ConnectionPool",
    "MySqlConnector.MySqlBulkCopy",
    "MySqlConnector.MySqlCommand",
    "MySqlConnector.MySqlConnection",
    "MySqlConnector.MySqlDataSource")]
