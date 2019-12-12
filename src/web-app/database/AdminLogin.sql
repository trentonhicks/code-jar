USE [RandomCode]
GO

CREATE TABLE [dbo].[AdminLogin](
	ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
	Username varchar(50) NOT NULL,
	[Password] varchar(50) NOT NULL,
)
