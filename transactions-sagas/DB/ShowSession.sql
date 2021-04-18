USE [Circus]
GO

/****** Object:  Table [dbo].[ShowSession]    Script Date: 18.04.2021 18:05:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShowSession](
	[ShowId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Price] [money] NOT NULL,
	[Currency] [varchar](5) NOT NULL
) ON [PRIMARY]
GO


