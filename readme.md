


##

Modes for the implementation


## Configuration fields

|Key|Required|Value|
|--|--|--|
|HeaderParam|no| Appends header information to the API requirest|
|OutputLocation|Conditional|Location where captured logs are stored. Depends on ConfigMode = Capture or FileCaptureOrCompare|
|ResultsStoreOption|Yes|Required with possible values None, FailuresOnly, All|
|UrlBase|Yes | Location where to make api call to|
|UrlParam|Yes|Url param allows adding query parameters to url|


### UrlParam

#### Static binding
----
populate the http request with a value to get a static parameter

```url
http://localhost:7055/WeatherForecast?urlkey=configKey
```
the config will look like this:

```c#
UrlBase = "http://localhost:7055",
UrlPath = "/WeatherForecast",
UrlParam = new List<Param>
            {
            new Param("urlKey", "configKey"),
            }
```


#### Data driven parameter

populate id with database

```url
http://localhost:7055/WeatherForecast?id=15
```

config to create the binding, mark the id from database in your sql query and use dbfields to capture the output

```c#
UrlBase = "http://localhost:7055",
UrlPath = "/WeatherForecast",
DBConnectionString = "<insert your connection string>",
 UrlParam = new List<Param>{
    new Param("urlKey", "configKey"),
    new Param("id", "bindingId")
},
DBQuery = "select id as bindingId from dbo.sampleTable;",
DBFields = new List<Param>{
    new Param("bindingId", "bindingId"),
    new Param("fieldName", "fieldName")
},
```

this will map to param with this pattern

```url
http://localhost:7055/WeatherForecast?id={bindingId}
```




### TesterConfigMode types

|Type|ConfigValue|UseCase|
|--|--|--|
|Run |1|Runs the tests only and shows result as overview.|
|Capture|2|Runs the tests and capture the results. Process will fail in case the file already exists. |
|FileCaptureOrCompare|3|Calls APIs and store result. If file already exists then it wil also compare output with api result.|
|APICompare|4|Not implemented yet. Realtime compare. Compares the results of two APIs. Good for regression testing of APIs.|

### ResultsStoreOption

this has to be used with configuration ConfigMode min level capture

|name|Numeric value| purpose|
|--|--|--|
|None|0|Just run the tests|
|FailuresOnly|1|Record only failures|
|All| 2|stores all results|


A path to store results file are using order in which data has been recorded in this application
```bash
/WeatherForecast?urlKey=configKey&id=1 404 fail Results/request-1.json
```

    
#### RUN

takes the configuration and only execute a call against the api
Reports success to the api as

Example of the test:
```bash
api base url: http://localhost:7071/

1.  /Data		OK success
2.  /Data/1		OK success
```


### ConfigMode

    /// <summary>
    /// Runs the tests only and shows result as overview.
    /// </summary>
    Run = 1,
    /// <summary>
    /// Runs the tests and capture the results.
    /// </summary>
    Capture = 2,
    /// <summary>
    /// Calls APIs and compare to a stored file.
    /// </summary>
    FileCompare = 3,
    /// <summary>
    /// Realtime compare. Compares the results of two APIs. 
    /// Good for regression testing of APIs.
    /// </summary>
    APICompare = 4


## Docker SQL test

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
```


```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=<YourStrong@Passw0rd>" -p 1433:1433 --name sql1 --hostname sql1  -d  mcr.microsoft.com/mssql/server:2022-latest
```


```SQL
USE [test]
GO
/****** Object:  Table [dbo].[sampleTable]    Script Date: 06/08/2023 02:46:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sampleTable](
	[id] [int] NOT NULL,
	[name] [nvarchar](50) NULL,
	[description] [nvarchar](max) NULL,
 CONSTRAINT [PK_sampleTable] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[sampleTable] ([id], [name], [description]) VALUES (1, N'Name', N'description')
GO
INSERT [dbo].[sampleTable] ([id], [name], [description]) VALUES (2, N'Name2', N'descriptopn2')
GO
INSERT [dbo].[sampleTable] ([id], [name], [description]) VALUES (3, N'Name3', N'description3')
GO
INSERT [dbo].[sampleTable] ([id], [name], [description]) VALUES (4, N'Name4', N'descirption 2')
GO
```



