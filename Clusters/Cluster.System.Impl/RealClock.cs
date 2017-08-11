using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.System.Api;

namespace Cluster.System.Impl
{
    /// <summary>
    /// This is the implementation of IClock that should be registered as a system service
    /// for normal operation; it merely wraps the DateTime static functions.
    /// </summary>
    public class RealClock : IClock
    {
        public DateTime Today() { return DateTime.Today; }

        public DateTime Now() {return DateTime.Now; }
        
    }
}
