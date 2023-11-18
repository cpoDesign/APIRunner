using APITestingRunner.Excetions;
using Dapper;
using Microsoft.Data.SqlClient;

namespace APITestingRunner.Database {
    public class DataAccess {
        private readonly Config config;

        public DataAccess(Config config) {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<IEnumerable<DataQueryResult>> FetchDataForRunnerAsync() {
            if (string.IsNullOrWhiteSpace(config.DBConnectionString)) throw new TestRunnerConfigurationErrorsException("Failed to load connection string");

            using SqlConnection connection = new(config.DBConnectionString);

            IEnumerable<object> result = await connection.QueryAsync<object>(config.DBQuery);

            List<DataQueryResult> list = new();
            int i = 0;
            foreach (object rows in result) {
                i++;

                DataQueryResult resultItem = new() { RowId = i, Results = new List<KeyValuePair<string, string>>() };

                IDictionary<string, object>? fieldsInResult = rows as IDictionary<string, object>;

                // get the fields from database and match to the object
                if(fieldsInResult is not null && config.DBFields is not null) { 
                    foreach (ConfigurationManager.Param configItem in config.DBFields) {
                        var fieldValue = fieldsInResult[configItem.Name]?.ToString()!;

                        resultItem.Results.Add(new KeyValuePair<string, string>(configItem.Name, fieldValue));
                    }

                    list.Add(resultItem);
                }
            }

            return list;
        }
    }
}
