using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.EntityFramework;
using Microsoft.EntityFrameworkCore;
using V1 = IkeMtz.Samples.Models.V1;

namespace IkeMtz.Samples.Data
{
  public class DatabaseContext : AuditableDbContext
  {
    public DatabaseContext(DbContextOptions<DatabaseContext> options, ICurrentUserProvider currentUserProvider)
        : base(options, currentUserProvider)
    {
    }

    public virtual DbSet<V1.Course> Courses { get; set; }
    public virtual DbSet<V1.SchoolCourse> SchoolCourses { get; set; }
    public virtual DbSet<V1.School> Schools { get; set; }
    public virtual DbSet<V1.StudentCourse> StudentCourses { get; set; }
    public virtual DbSet<V1.Student> Students { get; set; }
    public virtual DbSet<V1.StudentSchool> StudentSchools { get; set; }
  }
}
