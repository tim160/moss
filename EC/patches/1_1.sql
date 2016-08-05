ALTER TABLE [dbo].[country]  WITH CHECK ADD  CONSTRAINT [FK_Country_Statuses] FOREIGN KEY([status_id])
REFERENCES [dbo].[status] ([id])
GO
