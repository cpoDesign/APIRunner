using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SampleAPI.Controllers {
  [ApiController]
  [Route("[controller]")]
  public partial class DataController : ControllerBase {
    private readonly ILogger<DataController> _logger;
    public DataController(ILogger<DataController> logger) {
      _logger = logger;
    }


    [HttpGet(Name = "get")]
    public async Task<IActionResult> GetAsync(int id, string urlKey = "") {
      if (string.IsNullOrEmpty(urlKey)) {
        return BadRequest("Provide url key");
      }

      string connectionString = $"Server=127.0.0.1; Database=testdb; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True";

      using SqlConnection connection = new(connectionString);
      string sql = $"select * from dbo.sampleTable where id={id};";
      SampleData result = await connection.QuerySingleAsync<SampleData>(sql);
      return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("/get/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id) {

      string connectionString = $"Server=127.0.0.1; Database=testdb; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True";

      using SqlConnection connection = new(connectionString);
      string sql = $"select * from dbo.sampleTable where id={id};";
      SampleData result = await connection.QuerySingleAsync<SampleData>(sql);
      return result == null ? NotFound() : Ok(result);
    }
  }
}
