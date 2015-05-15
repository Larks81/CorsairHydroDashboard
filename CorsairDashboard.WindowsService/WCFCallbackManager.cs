using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.WindowsService
{
    public class WCFCallbackManager<TGroup, ICallback> : IDisposable where ICallback : class
    {
        private volatile Dictionary<TGroup, List<ICallback>> callbacks;

        public WCFCallbackManager()
        {
            callbacks = new Dictionary<TGroup, List<ICallback>>();
        }

        public void SubscribeClientForGroup(TGroup group, ICallback client)
        {
            lock (callbacks)
            {
                OperationContext.Current.Channel.Closed += OnChannelClosedOrFaulted;
                OperationContext.Current.Channel.Faulted += OnChannelClosedOrFaulted;

                List<ICallback> callbacksForGroup;
                if (callbacks.TryGetValue(group, out callbacksForGroup))
                {
                    if (!callbacksForGroup.Contains(client))
                    {
                        callbacksForGroup.Add(client);
                    }
                }
                else
                {
                    callbacks[group] = new List<ICallback>() { client };
                }
            }
        }

        public void UnsubscribeClientFromGroup(TGroup group, ICallback client)
        {
            lock (callbacks)
            {
                List<ICallback> callbacksForGroup;
                if (callbacks.TryGetValue(group, out callbacksForGroup))
                {
                    callbacksForGroup.Remove(client);
                }
            }
        }

        public bool HasAnyClientForGroup(TGroup group)
        {
            List<ICallback> callbacksForGroup;
            if (callbacks.TryGetValue(group, out callbacksForGroup))
            {
                return callbacksForGroup.Any();
            }
            return false;
        }

        public void NotifyAllClientsOfGroup(TGroup group, Action<ICallback> notificationFunction)
        {
            lock (callbacks)
            {
                List<ICallback> callbacksForGroup;
                if (callbacks.TryGetValue(group, out callbacksForGroup))
                {
                    foreach (var callback in callbacksForGroup)
                    {
                        try
                        {
                            notificationFunction(callback);
                        }
                        catch { }
                    }
                }
            }
        }

        private void OnChannelClosedOrFaulted(object sender, EventArgs e)
        {
            var callback = sender as ICallback;
            if (callback != null)
            {
                lock (callbacks)
                {
                    var lists = callbacks.Where(kvp => kvp.Value.Contains(callback)).Select(kvp => kvp.Value);
                    foreach (var callbacksList in lists)
                    {
                        callbacksList.Remove(callback);
                    }
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Internal variable which checks if Dispose has already been called
        /// </summary>
        private Boolean disposed;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(Boolean disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                lock (callbacks)
                {
                    callbacks.Clear();
                }                
            }

            disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Call the private Dispose(bool) helper and indicate 
            // that we are explicitly disposing
            this.Dispose(true);

            // Tell the garbage collector that the object doesn't require any
            // cleanup when collected since Dispose was called explicitly.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The destructor for the class.
        /// </summary>
        ~WCFCallbackManager()
        {
            this.Dispose(false);
        }

        #endregion
    }
}
