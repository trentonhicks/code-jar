USE [Random-Code]
GO

/****** Object:  Table [dbo].[6 digit code]    Script Date: 11/22/2019 8:41:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[6 digit code](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SeedValue] [int] NOT NULL,
	[StringValue] [varchar](6) NOT NULL,
	[State] [varchar](50) NOT NULL,
	[DateActive] [datetime] NOT NULL,
	[DateExpires] [datetime] NOT NULL,
 CONSTRAINT [PK_6 digit code] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

