using System.Data.Entity;
using Cluster.Tasks.Api;
using Cluster.Tasks.Impl;
using Cluster.Forms.Impl;
using Cluster.Addresses.Impl;
using Cluster.Addresses.Test;
using Cluster.Names.Test;
using Cluster.Tasks.Test;
using Cluster.Countries.Test;
using Cluster.Telephones.Test;
using Cluster.Emails.Test;
using Cluster.Documents.Test;
using Cluster.Forms.Test;
using Cluster.Accounts.Test;
//using Cluster.Batch.Test;
using Cluster.Users.Test;
using Template.DataBase;

namespace Template.SeedData
{
    public class AppInitializer :  DropCreateDatabaseIfModelChanges<AppDbContext> 
    {
        private AppDbContext _context;

        protected override void Seed(AppDbContext context)
        {
            _context = context;

            #region Users and Roles
            //UsersTestInitializer.TestRoles(context);
            //UsersTestInitializer.TestUsers(context, context);

            #endregion

            #region Scheduling
            //SchedulingTestInitializer.TestProcessDefinitions(context);
            //SchedulingTestInitializer.NewProcessDefinition(context.BatchProcessDefinitions, "Unsuspend Tasks", typeof(UnsuspendTasksBatchProcess));
            //SchedulingTestInitializer.NewProcessDefinition(context.BatchProcessDefinitions, "Generate Reminder: Create Monthly Reports", typeof(CreateTaskBatchProcess), 1);
            //SchedulingTestInitializer.NewProcessDefinition(context.BatchProcessDefinitions, "Generate Reminder: Chase overdue debtors", typeof(CreateTaskBatchProcess), 2);
            #endregion

            #region Tasks
            //TasksTestInitializer.AllTaskTypes(context);
            //TasksTestInitializer.Tasks(context);
            //TasksTestInitializer.CreateTaskScheduledProcesses(context);
            #endregion

            CountriesTestInitializer.AllCountries(context);
            TelephonesTestInitializer.AllTelephoneCodes(context);
            EmailsTestInitializer.AllEmailDetails(context);

            NamesTestInitializer.TestNames(context);

            //DocumentsTestInitializer.AllNotes(context);

            #region Scheduled Processes
            //TasksTestInitializer.CreateTaskScheduledProcesses(context);
            #endregion
        }
		
        private CreateTaskBatchProcess NewCreateTaskBatchProcess(string text)
        {
            var crt = new CreateTaskBatchProcess()
            {
                NotesToAddToTask = text
            };
            _context.CreateTaskBatchProcesses.Add(crt);
            return crt;
        }  
    }
}
