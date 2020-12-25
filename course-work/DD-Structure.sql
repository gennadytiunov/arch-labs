USE [EMS-Feat]
GO
/****** Object:  Table [dbo].[Booking]    Script Date: 29.11.2020 14:35:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Booking](
	[Id] [int] NOT NULL,
	[ShowId] [int] NOT NULL,
	[SessionDate] [datetime] NOT NULL,
	[Amount] [money] NOT NULL,
	[Contact] [nvarchar](500) NOT NULL,
	[Card] [varchar](100) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
	[UpdateDate] [datetime] NULL,
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BookingSeat]    Script Date: 29.11.2020 14:35:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BookingSeat](
	[BookingId] [int] NOT NULL,
	[Row] [int] NOT NULL,
	[Number] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HallSeat]    Script Date: 29.11.2020 14:35:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HallSeat](
	[Row] [int] NOT NULL,
	[Number] [int] NOT NULL,
 CONSTRAINT [PK_HallSeat] PRIMARY KEY CLUSTERED 
(
	[Row] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Show]    Script Date: 29.11.2020 14:35:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Show](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_Show] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ShowSession]    Script Date: 29.11.2020 14:35:12 ******/
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
ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Show] FOREIGN KEY([ShowId])
REFERENCES [dbo].[Show] ([Id])
GO
ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Show]
GO
ALTER TABLE [dbo].[ShowSession]  WITH CHECK ADD  CONSTRAINT [FK_ShowSession_Show] FOREIGN KEY([ShowId])
REFERENCES [dbo].[Show] ([Id])
GO
ALTER TABLE [dbo].[ShowSession] CHECK CONSTRAINT [FK_ShowSession_Show]
GO
