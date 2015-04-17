using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorsairDashboard.HydroDataProvider
{
    internal class TaskCachedResult<T>
    {
        private T value, nullValue;
        private Task<T> originalTask;

        public Task<T> Value {
            get
            {                                
                if (value == null || value.Equals(nullValue))
                {
                    return originalTask;
                }
                else
                {
                    return Task.FromResult(value);
                }
            }
        }

        public TaskCachedResult(Task<T> task, T nullValue = default(T))
        {
            this.nullValue = nullValue;
            originalTask = task;
            originalTask.ContinueWith(t => value = t.Result);
        }
    }
}
