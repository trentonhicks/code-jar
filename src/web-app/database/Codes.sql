USE [RandomCode]
GO
CREATE TABLE [dbo].[Codes](
    ID int PRIMARY KEY IDENTITY(1,1) NOT NULL,
    SeedValue int UNIQUE NOT NULL,
    [State] tinyint NOT NULL);
   