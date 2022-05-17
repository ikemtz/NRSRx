/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\dbo\Data\Courses.sql
:r .\dbo\Data\Schools.sql
:r .\dbo\Data\Students.sql
:r .\dbo\Data\SchoolCourses.sql
:r .\dbo\Data\StudentSchools.sql
:r .\dbo\Data\StudentCourses.sql