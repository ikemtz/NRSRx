CREATE TABLE [dbo].[Courses] (
    [Id]           UNIQUEIDENTIFIER   CONSTRAINT [DF_Courses_Id] DEFAULT (newid()) NOT NULL,
    [Department]   VARCHAR (150)      NOT NULL,
    [Num]          VARCHAR (10)       NOT NULL,
    [Title]        VARCHAR (150)      NOT NULL,
    [Description]  VARCHAR (500)      NULL,
    [PassRate]     FLOAT (53)         NULL,
    [AvgScore]     FLOAT (53)         NULL,
    [CreatedBy]    VARCHAR (250)      NOT NULL,
    [CreatedOnUtc] DATETIMEOFFSET (7) CONSTRAINT [DF_Courses_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdatedBy]    VARCHAR (250)      NULL,
    [UpdatedOnUtc] DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_Courses] PRIMARY KEY CLUSTERED ([Id] ASC)
);









