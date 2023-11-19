﻿// See https://aka.ms/new-console-template for more information

using APITestingRunner.Database;

namespace APITestingRunner.ApiRequest {
  public class DataRequestConstructor {
    private string _baseUrl = string.Empty;
    private string _relativeUrl = string.Empty;

    /// <summary>
    /// Uset to compose url based on dataquery result. Used to create data driven URL
    /// </summary>
    /// <param name="urlPath">Endpoint url|/param>
    /// <param name="_config">Instance of configuration parameters</param>
    /// <param name="dbData">Data from database used to merge to create a data driven url</param>
    /// <returns>Instance of object</returns>
    public DataRequestConstructor ComposeUrlAddressForRequest(string urlPath, Config _config, DataQueryResult? dbData) {
      if (!string.IsNullOrWhiteSpace(urlPath)) {
        _relativeUrl = urlPath;
      }

      if (_config.UrlParam == null || _config.UrlParam.Count() == 0) {
        return this;
      }

      bool isFirst = true;

      foreach (Param item in _config.UrlParam) {
        if (isFirst) {
          isFirst = false;
          urlPath += $"?{item.Name}=";
        } else {
          urlPath += $"&{item.Name}=";
        }

        //check if item value is listed in dbfields, if yes we have mapping to value from database otherwise  just use value
        if (_config.DBFields != null) {
          if (_config.DBFields.Any(x => x.value == item.value) && dbData != null) {
            //replace value from dbData object
            KeyValuePair<string, string> dbResultFound = dbData.Results.FirstOrDefault(x => x.Key == item.value);

            urlPath += $"{dbResultFound.Value}";

          } else {
            // no match found in parameters 
            urlPath += $"{item.value}";
          }
        } else {
          // no match found in parameters 
          urlPath += $"{item.value}";
        }
      }

      _relativeUrl = urlPath;
      return this;
    }

    public DataRequestConstructor AddBaseUrl(string urlBase) {
      _baseUrl = urlBase;

      return this;
    }

    public string GetPathAndQuery() {
      return _relativeUrl;
    }

    public string? GetFullUrl() {
      return $"{_baseUrl}{_relativeUrl}";
    }
  }
}