using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SampleAPI.Models;

namespace SampleAPI.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public partial class DataModelController : ControllerBase
  {
    private readonly ILogger<DataController> _logger;
    public DataModelController(ILogger<DataController> logger)
    {
      _logger = logger;
    }


    [HttpGet("/DataModel")]
    public async Task<IActionResult> GetFromBodyAsync([FromBody] RequestFromBodyModel model, [FromQuery] string urlKey = "")
    {
      if (string.IsNullOrEmpty(urlKey))
      {
        return BadRequest("Provide url key");
      }

      if (string.IsNullOrWhiteSpace(model.StaticData))
      {
        return NotFound();
      }

      string connectionString = $"Server=127.0.0.1; Database=test; User Id=sa; Password=<YourStrong@Passw0rd>;TrustServerCertificate=True";

      using SqlConnection connection = new(connectionString);
      string sql = $"select * from dbo.sampleTable where id={model.Id};";
      SampleData result = await connection.QuerySingleAsync<SampleData>(sql);
      return result == null ? NotFound() : Ok(result);
    }
  }
}
