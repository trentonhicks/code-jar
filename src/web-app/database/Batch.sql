USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    [ID] int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[BatchName] varchar(50) NOT NULL,
	[CodeIDStart] int unique NOT NULL,
	[CodeIDEnd] AS [CodeIDStart] + [BatchSize] - 1,
    [BatchSize] int NOT NULL,
	[DateActive] DATETIME NOT NULL,
	[DateExpires] DATETIME NOT NULL)