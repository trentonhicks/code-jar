USE [RandomCode]
GO

CREATE TABLE [dbo].[Codes](
	ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	SeedValue int UNIQUE NOT NULL,
	[State] varchar(50) NOT NULL,
	DateActive datetime NOT NULL,
	DateExpires datetime NOT NULL,
)