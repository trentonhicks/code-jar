USE [RandomCode]
GO
CREATE TABLE [dbo].[Batch](
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	BatchStart int FOREIGN KEY REFERENCES Codes(ID) NOT NULL,
	BatchEnd int FOREIGN KEY REFERENCES Codes(ID) NOT NULL,
    [BatchSize] AS BatchEnd - BatchStart + 1, 
	DateActive DATETIME NOT NULL,
	DateExpires DATETIME NOT NULL)