ALTER TABLE [dbo].[company_department]  WITH CHECK ADD  CONSTRAINT [FK_Department_Company] FOREIGN KEY([company_id])
REFERENCES [dbo].[company] ([id])
GO



