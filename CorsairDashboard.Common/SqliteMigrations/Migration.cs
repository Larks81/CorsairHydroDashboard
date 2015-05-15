using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Common.SqliteMigrations
{
    public abstract class Migration
    {
        private List<String> statements = new List<string>();

        public abstract uint Version { get; }

        public IReadOnlyCollection<String> Statements
        {
            get
            {
                return statements.AsReadOnly();
            }
        }

        protected void AddStatement(String stmt)
        {
            if (!String.IsNullOrEmpty(stmt))
            {
                statements.Add(stmt);
            }
        }
    }
}
