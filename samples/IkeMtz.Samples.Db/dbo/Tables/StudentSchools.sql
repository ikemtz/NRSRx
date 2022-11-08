CREATE TABLE [dbo].[StudentSchools] (
    [Id]             UNIQUEIDENTIFIER   CONSTRAINT [DF_StudentSchools_Id] DEFAULT (newid()) NOT NULL,
    [SchoolId]       UNIQUEIDENTIFIER   NOT NULL,
    [StudentId]      UNIQUEIDENTIFIER   NOT NULL,
    [TenantId]       VARCHAR (5)        NOT NULL,
    [EnrollmentDate] DATE               NOT NULL,
    [CreatedBy]      VARCHAR (250)      NOT NULL,
    [CreatedOnUtc]   DATETIMEOFFSET (7) CONSTRAINT [DF_StudentSchools_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdateCount]  INT NULL,
    [UpdatedBy]      VARCHAR (250)      NULL,
    [UpdatedOnUtc]   DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_StudentSchools] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudentSchools_Schools] FOREIGN KEY ([SchoolId]) REFERENCES [dbo].[Schools] ([Id]),
    CONSTRAINT [FK_StudentSchools_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id])
);




