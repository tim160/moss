ALTER TABLE [dbo].[report_non_mediator_involved]  WITH CHECK ADD  CONSTRAINT [FK_ReportNonMediatorInvolved_Report] FOREIGN KEY([report_id])
REFERENCES [dbo].[report] ([id])
GO


ALTER TABLE [dbo].[report]  WITH CHECK ADD  CONSTRAINT [FK_Report_ManagementKnow] FOREIGN KEY([management_know_id])
REFERENCES  [dbo].[management_know] ([id])
GO

ALTER TABLE [dbo].[report]  WITH CHECK ADD  CONSTRAINT [FK_Report_User] FOREIGN KEY([user_id])
REFERENCES  [dbo].[user] ([id])
GO
