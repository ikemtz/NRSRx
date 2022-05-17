CREATE TABLE [dbo].[Students] (
    [Id]           UNIQUEIDENTIFIER   CONSTRAINT [DF_Students_Id] DEFAULT (newid()) NOT NULL,
    [Title]        NVARCHAR (50)      NULL,
    [FirstName]    NVARCHAR (250)     NOT NULL,
    [LastName]     NVARCHAR (250)     NOT NULL,
    [MiddleName]   NVARCHAR (250)     NULL,
    [BirthDate]    DATE               NOT NULL,
    [Gender]       INT                NOT NULL,
    [Email]        VARCHAR (250)      NOT NULL,
    [Tel1]         VARCHAR (15)       NULL,
    [Tel2]         VARCHAR (15)       NULL,
    [CreatedBy]    VARCHAR (250)      NOT NULL,
    [CreatedOnUtc] DATETIMEOFFSET (7) CONSTRAINT [DF_Students_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdatedBy]    VARCHAR (250)      NULL,
    [UpdatedOnUtc] DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED ([Id] ASC)
);








