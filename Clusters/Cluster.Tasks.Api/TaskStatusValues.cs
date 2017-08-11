using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.Tasks.Api
{
    public enum TaskStatusValues
    {
        Any = 0,
        Unsaved = 1,
        Pending = 2,
        Started = 3,
        Completed = 4,
        Cancelled = 5,
        Suspended = 6
    }
}
