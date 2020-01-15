USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BatchName varchar(50) NOT NULL,
	CodeIDStart int NOT NULL,
	CodeIDEnd int NOT NULL,
    [BatchSize] AS CodeIDEnd - CodeIDStart + 1,
	DateActive DATETIME NOT NULL,
	DateExpires DATETIME NOT NULL)