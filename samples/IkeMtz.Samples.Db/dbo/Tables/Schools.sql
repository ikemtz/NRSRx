CREATE TABLE [dbo].[Schools] (
    [Id]           UNIQUEIDENTIFIER   CONSTRAINT [DF_Schools_Id] DEFAULT (newid()) NOT NULL,
    [Name]         VARCHAR (50)       NOT NULL,
    [FullName]     VARCHAR (250)      NOT NULL,
    [TenantId]     VARCHAR (5)        NOT NULL,
    [CreatedBy]    VARCHAR (250)      NOT NULL,
    [CreatedOnUtc] DATETIMEOFFSET (7) CONSTRAINT [DF_Schools_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdatedBy]    VARCHAR (250)      NULL,
    [UpdatedOnUtc] DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_Schools] PRIMARY KEY CLUSTERED ([Id] ASC)
);







GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_School_TenantId]
    ON [dbo].[Schools]([TenantId] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_School_Name]
    ON [dbo].[Schools]([Name] ASC);

