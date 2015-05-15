using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Common.SqliteMigrations
{
    public class SqliteMigrationsInitializer<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext, ISqliteMigrationsSupport
    {
        public void InitializeDatabase(TContext context)
        {
            context.Database.CreateIfNotExists();
            context.Database.ExecuteSqlCommand("CREATE TABLE IF NOT EXISTS MigrationsData(Version NUMERIC, Statements TEXT)");

            //check if migrations are needed
            var result = context.Database.SqlQuery<int>("SELECT Version FROM MigrationsData ORDER BY Version");
            var appliedMigrations = result.ToList();
            var migrationsToBeApplied = context.Migrations.Where(m => !appliedMigrations.Contains((int)m.Version)).OrderBy(m => m.Version);
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var migration in migrationsToBeApplied)
                    {
                        var stmt = String.Join("; ", migration.Statements);
                        context.Database.ExecuteSqlCommand(stmt);
                        context.Database.ExecuteSqlCommand("INSERT INTO MigrationsData VALUES (@p0, @p1)", migration.Version, stmt);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
