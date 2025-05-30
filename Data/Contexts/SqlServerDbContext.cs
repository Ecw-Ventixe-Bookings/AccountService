

using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class SqlServerDbContext(DbContextOptions opt) : DbContext(opt)
{
    public DbSet<AccountEntity> Accounts { get; set; }
}
