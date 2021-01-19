USE [Circus]
GO

/****** Object:  Table [dbo].[ShowSession]    Script Date: 19.01.2021 1:05:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShowSession](
	[ShowId] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[Price] [money] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ShowSession]  WITH CHECK ADD  CONSTRAINT [FK_ShowSession_Show] FOREIGN KEY([ShowId])
REFERENCES [dbo].[Show] ([Id])
GO

ALTER TABLE [dbo].[ShowSession] CHECK CONSTRAINT [FK_ShowSession_Show]
GO

