

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

