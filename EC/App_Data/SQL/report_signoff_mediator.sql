USE [EC]
GO

/****** Object:  Table [dbo].[report_owner]    Script Date: 21.05.2018 10:00:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[report_signoff_mediator](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[report_id] [int] NOT NULL,
	[user_id] [int] NOT NULL,
	[created_on] [datetime] NOT NULL,
	[status_id] [int] NOT NULL,
	[createdby_user_id] [int] NOT NULL,
 CONSTRAINT [PK_report_signoff_mediator] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[report_signoff_mediator] ADD  CONSTRAINT [DF_report_signoff_mediator_created_on]  DEFAULT (getdate()) FOR [created_on]
GO

ALTER TABLE [dbo].[report_signoff_mediator] ADD  CONSTRAINT [DF_report_signoff_mediator_status_id]  DEFAULT ((1)) FOR [status_id]
GO

ALTER TABLE [dbo].[report_signoff_mediator]  WITH CHECK ADD  CONSTRAINT [FK_report_signoff_mediator_user_id] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO

ALTER TABLE [dbo].[report_signoff_mediator]  WITH CHECK ADD  CONSTRAINT [FK_report_signoff_mediator_createdby_user_id] FOREIGN KEY([user_id])
REFERENCES [dbo].[user] ([id])
GO

ALTER TABLE [dbo].[report_signoff_mediator]  WITH CHECK ADD  CONSTRAINT [FK_report_signoff_mediator_report_id] FOREIGN KEY([report_id])
REFERENCES [dbo].[report] ([id])
GO
