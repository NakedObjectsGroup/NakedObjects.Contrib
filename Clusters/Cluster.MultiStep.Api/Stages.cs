﻿
namespace Cluster.MultiStep.Impl
{
    /// <summary>
    /// The Stage controls the availability of sequence methods such as Next, Previous.
    /// It is set externally so that the same Step may be re-used at differing stages 
    /// of the process within multiple activity types.
    /// </summary>
    public enum Stages
    {
        Start, InProcess, End
    }
}
