using NakedObjects;

namespace Cluster.Tasks.Api
{
    public interface ITaskAssignee : IHasIntegerId
    {
        string Name {get;}
    }
}
