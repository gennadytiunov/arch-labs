USE [Circus]
GO

/****** Object:  Table [dbo].[Transaction]    Script Date: 19.01.2021 1:05:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Transaction](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [int] NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Details] [nvarchar](max) NOT NULL,
	[Amount] [money] NOT NULL,
	[Currency] [varchar](5) NOT NULL,
	[Date] [datetime] NOT NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

