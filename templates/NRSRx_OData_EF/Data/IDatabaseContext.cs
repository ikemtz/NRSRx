using Microsoft.EntityFrameworkCore;
using NRSRx_OData_EF.Models;

namespace NRSRx_OData_EF.Data
{
  public interface IDatabaseContext
  {
    DbSet<Item> Items { get; set; }
  }
}

