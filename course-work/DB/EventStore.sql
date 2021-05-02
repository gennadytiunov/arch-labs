USE [Circus]
GO

/****** Object:  Table [dbo].[EventStore]    Script Date: 26.04.2021 20:41:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EventStore](
	[EventId] [uniqueidentifier] NOT NULL,
	[EntityType] [varchar](50) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[EventType] [varchar](50) NOT NULL,
	[EventPayload] [nvarchar](max) NOT NULL,
	[EventDate] [datetime] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


