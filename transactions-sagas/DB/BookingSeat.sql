USE [Circus]
GO

/****** Object:  Table [dbo].[BookingSeat]    Script Date: 19.01.2021 1:09:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BookingSeat](
	[BookingId] [uniqueidentifier] NOT NULL,
	[Row] [int] NOT NULL,
	[Number] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[BookingSeat]  WITH CHECK ADD  CONSTRAINT [FK_BookingSeat_Booking] FOREIGN KEY([BookingId])
REFERENCES [dbo].[Booking] ([Id])
GO

ALTER TABLE [dbo].[BookingSeat] CHECK CONSTRAINT [FK_BookingSeat_Booking]
GO

