using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace Cluster.Tasks.Api
{
    public static class TasksRoles
    {
        public const string TaskAssignee = "Task Assignee";
        public const string TaskCreator = "Task Creator";

        public static bool IsTaskAssignee(this IPrincipal principal)
        {
            return principal.IsInRole(TaskAssignee);
        }

        public static bool IsTaskCreator(this IPrincipal principal)
        {
            return principal.IsInRole(TaskCreator);
        }
    }
}
