using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cluster.System.Api
{
    /// <summary>
    /// Common point for accessing the clock, to allow it to be mocked for testing
    /// </summary>
    public interface IClock
    {
        DateTime Today();

            DateTime Now();
    }
}
