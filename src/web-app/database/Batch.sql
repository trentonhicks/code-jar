USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    [ID] int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[BatchName] varchar(50) NOT NULL,
    [BatchSize] int NOT NULL,
    [State] tinyint NOT NULL,
	[DateActive] DATETIME NOT NULL,
	[DateExpires] DATETIME NOT NULL)