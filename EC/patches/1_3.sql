ALTER TABLE [dbo].[company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Address] FOREIGN KEY([address_id])
REFERENCES [dbo].[address] ([id])
GO

ALTER TABLE [dbo].[address]  WITH CHECK ADD  CONSTRAINT [FK_Address_Country] FOREIGN KEY([country_id])
REFERENCES [dbo].[country] ([id])
GO
