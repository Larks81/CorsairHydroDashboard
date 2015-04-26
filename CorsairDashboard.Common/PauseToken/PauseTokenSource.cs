using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CorsairDashboard.Common.PauseToken
{
    public class PauseTokenSource
    {
        private volatile TaskCompletionSource<bool> paused;
        internal static readonly Task CompletedTask = Task.FromResult(true);

        public bool IsPaused
        {
            get { return paused != null; }
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(ref paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        public PauseToken PauseToken
        {
            get
            {
                return new PauseToken(this);
            }
        }

        internal Task WaitWhilePausedAsync()
        {
            var cur = paused;
            return cur != null ? cur.Task : CompletedTask;
        }
    }
}
