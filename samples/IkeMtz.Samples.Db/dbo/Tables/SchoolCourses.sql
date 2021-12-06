CREATE TABLE [dbo].[SchoolCourses] (
    [Id]           UNIQUEIDENTIFIER   CONSTRAINT [DF_SchoolCourses_Id] DEFAULT (newid()) NOT NULL,
    [SchoolId]     UNIQUEIDENTIFIER   NOT NULL,
    [CourseId]     UNIQUEIDENTIFIER   NOT NULL,
    [PassRate]     FLOAT (53)         NOT NULL,
    [AvgScore]     FLOAT (53)         NOT NULL,
    [CreatedBy]    VARCHAR (250)      NOT NULL,
    [CreatedOnUtc] DATETIMEOFFSET (7) CONSTRAINT [DF_SchoolCourses_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdatedBy]    VARCHAR (250)      NULL,
    [UpdatedOnUtc] DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_SchoolCourses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SchoolCourses_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id]),
    CONSTRAINT [FK_SchoolCourses_Schools] FOREIGN KEY ([SchoolId]) REFERENCES [dbo].[Schools] ([Id])
);



