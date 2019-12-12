USE [RandomCode]
GO

CREATE TABLE [dbo].[Batch](
	ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BatchValue  bigint UNIQUE NOT NULL,
)