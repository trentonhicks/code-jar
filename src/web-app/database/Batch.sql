USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	StartDate datetime NOT NUll,
	[State] varchar(50) NOT NULL,
    BatchValue int DEFAULT 0 NOT NULL);
INSERT INTO Batch (BatchValue)
    VALUES (0);