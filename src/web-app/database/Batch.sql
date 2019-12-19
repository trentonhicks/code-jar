USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BatchName varchar(50) NOT NULL,
	CodeIDStart int FOREIGN KEY REFERENCES Codes(ID) NOT NULL,
	CodeIDEnd int FOREIGN KEY REFERENCES Codes(ID) NOT NULL,
    [BatchSize] AS CodeIDStart - CodeIDEnd + 1, 
	DateActive DATETIME NOT NULL,
	DateExpires DATETIME NOT NULL)