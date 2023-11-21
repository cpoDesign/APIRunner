using APITestingRunner.Excetions;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace APITestingRunner.Database
{
	public class DataAccess
	{
		private readonly Config _config;
		private readonly ILogger _logger;

		public DataAccess(Config config, ILogger logger)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<IEnumerable<DataQueryResult>> FetchDataForRunnerAsync()
		{

			if (string.IsNullOrWhiteSpace(_config.DBConnectionString)) throw new TestRunnerConfigurationErrorsException("Failed to load connection string");

			List<DataQueryResult> list = new();

			try
			{
				_logger.LogDebug($"Attempting to use connection string: {_config.DBConnectionString}");

				using SqlConnection connection = new(_config.DBConnectionString);

				var result = await connection.QueryAsync<object>(_config.DBQuery);

				var i = 0;
				foreach (var rows in result)
				{
					i++;

					DataQueryResult resultItem = new() { RowId = i, Results = new List<KeyValuePair<string, string>>() };

					var fieldsInResult = rows as IDictionary<string, object>;

					// get the fields from database and match to the object
					if (fieldsInResult is not null && _config.DBFields is not null)
					{
						foreach (var configItem in _config.DBFields)
						{
							var fieldValue = fieldsInResult[configItem.Name]?.ToString()!;

							resultItem.Results.Add(new KeyValuePair<string, string>(configItem.Name, fieldValue));
						}

						list.Add(resultItem);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				throw;
			}

			return list;
		}
	}
}
