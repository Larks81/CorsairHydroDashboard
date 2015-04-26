using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.Common.PauseToken
{
    public struct PauseToken
    {
        private readonly PauseTokenSource source;

        public bool IsPaused
        {
            get
            {
                return source != null && source.IsPaused;
            }
        }

        internal PauseToken(PauseTokenSource source)
        {
            this.source = source;
        }
        
        public Task WaitWhilePausedAsync()
        {
            return IsPaused ? source.WaitWhilePausedAsync() : PauseTokenSource.CompletedTask;
        }
    }
}
