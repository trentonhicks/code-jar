USE [RandomCode]
GO

CREATE TABLE [dbo].[Batch](
	ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BatchValue int NOT NULL,)

INSERT INTO Batch (ID, BatchValue)
	VALUES (1,0);