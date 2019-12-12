USE [RandomCode]
GO

CREATE TABLE [dbo].[Offset](
	ID int PRIMARY KEY NOT NULL,
	OffsetValue bigint DEFAULT 0 NOT NULL,
)