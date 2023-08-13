// See https://aka.ms/new-console-template for more information

namespace APITestingRunner
{
  public class DataRequestConstructor
  {
    public string? ComposeUrlAddressForRequest(string urlPath, Config _config, DataQueryResult? dbData)
    {
      if (_config.UrlParam == null || _config.UrlParam.Count() == 0)
      {
        return urlPath;
      }

      bool isFirst = true;

      foreach (ConfigurationManager.Param item in _config.UrlParam)
      {
        if (isFirst)
        {
          isFirst = false;
          urlPath += $"?{item.Name}=";
        }
        else
        {
          urlPath += $"&{item.Name}=";
        }

        //check if item value is listed in dbfields, if yes we have mapping to value from database otherwise  just use value
        if (_config.DBFields != null)
        {
          if (_config.DBFields.Any(x => x.value == item.value) && dbData != null)
          {
            //replace value from dbData object
            KeyValuePair<string, string> dbResultFound = dbData.Results.FirstOrDefault(x => x.Key == item.value);

            urlPath += $"{dbResultFound.Value}";

          }
          else
          {
            // no match found in parameters 
            urlPath += $"{item.value}";
          }
        }
        else
        {
          // no match found in parameters 
          urlPath += $"{item.value}";
        }
      }

      return urlPath;
    }
  }
}