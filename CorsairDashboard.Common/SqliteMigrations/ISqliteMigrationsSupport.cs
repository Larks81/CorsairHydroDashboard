using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Common.SqliteMigrations
{
    public interface ISqliteMigrationsSupport
    {
        IEnumerable<Migration> Migrations { get; }
    }
}
