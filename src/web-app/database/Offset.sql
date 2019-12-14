USE [RandomCode]
GO

CREATE TABLE [dbo].[Offset](
	ID int PRIMARY KEY NOT NULL,
	OffsetValue bigint DEFAULT 0 NOT NULL,);

	INSERT INTO Offset (ID, OffsetValue)
	VALUES (1,0);
