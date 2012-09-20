CREATE TABLE [dbo].[GroupCategoryAspect]
(
[InstanceId] [uniqueidentifier] NOT NULL,
[Id] [uniqueidentifier] NOT NULL DEFAULT (newid()),
[Name] [varchar] (75) NOT NULL,
[ParentId] [uniqueidentifier] NULL

);
