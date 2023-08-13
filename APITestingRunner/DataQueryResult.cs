namespace APITestingRunner
{
  public class DataQueryResult
  {
    public int RowId { get; set; } = 0;
    public List<KeyValuePair<string, string>> Results = new();
  }
}
