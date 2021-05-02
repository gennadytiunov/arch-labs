USE [Circus]
GO

/****** Object:  Table [dbo].[Booking]    Script Date: 18.04.2021 18:04:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Booking](
	[Id] [uniqueidentifier] NOT NULL,
	[PaymentId] [uniqueidentifier] NULL,
	[UserId] [int] NOT NULL,
	[ShowId] [int] NOT NULL,
	[SessionDate] [datetime] NOT NULL,
	[Amount] [money] NOT NULL,
	[Currency] [varchar](5) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[SubStatus] [varchar](50) NULL,
	[CreationDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Show] FOREIGN KEY([ShowId])
REFERENCES [dbo].[Show] ([Id])
GO

ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Show]
GO


