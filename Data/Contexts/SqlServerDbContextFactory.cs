

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace Data.Contexts;

internal class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
{
    public SqlServerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
       .AddUserSecrets<SqlServerDbContextFactory>()
       .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("accountConnection"));

        return new SqlServerDbContext(optionsBuilder.Options);
    }
}
