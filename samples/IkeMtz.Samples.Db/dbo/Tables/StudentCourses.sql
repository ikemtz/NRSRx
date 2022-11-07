CREATE TABLE [dbo].[StudentCourses] (
    [Id]           UNIQUEIDENTIFIER   CONSTRAINT [DF_StudentCourses_Id] DEFAULT (newid()) NOT NULL,
    [StudentId]    UNIQUEIDENTIFIER   NOT NULL,
    [SchoolId]     UNIQUEIDENTIFIER   NOT NULL,
    [CourseId]     UNIQUEIDENTIFIER   NOT NULL,
    [SchoolCourseId]     UNIQUEIDENTIFIER   NULL,
    [Semester]     VARCHAR (50)       NOT NULL,
    [Year]         INT                NOT NULL,
    [FinalScore]   FLOAT (53)         NOT NULL,
    [CreatedBy]    VARCHAR (250)      NOT NULL,
    [CreatedOnUtc] DATETIMEOFFSET (7) CONSTRAINT [DF_StudentCourses_CreatedOnUtc] DEFAULT (getutcdate()) NOT NULL,
    [UpdateCount]  INT NULL,
    [UpdatedBy]    VARCHAR (250)      NULL,
    [UpdatedOnUtc] DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_StudentCourses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudentCourses_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([Id]),
    CONSTRAINT [FK_StudentCourses_Schools] FOREIGN KEY ([SchoolId]) REFERENCES [dbo].[Schools] ([Id]),
    CONSTRAINT [FK_StudentCourses_Students] FOREIGN KEY ([StudentId]) REFERENCES [dbo].[Students] ([Id]),
    CONSTRAINT [FK_StudentCourses_SchoolCourses] FOREIGN KEY ([SchoolCourseId]) REFERENCES [dbo].[SchoolCourses] ([Id])
);





