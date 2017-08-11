using System.Data.Entity;
using App.ScheduledProcessRunner;
using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Snapshot;
using Cluster.Tasks.Impl;
using Cluster.Forms.Impl;
using Cluster.Users;
using Cluster.Addresses.Impl;
using Cluster.Tasks.Test;
using Cluster.Names.Impl;
using Cluster.Addresses.Test;
using Cluster.Documents.Impl;
using Cluster.Documents.Test;
using Cluster.Emails.Test;
using Cluster.Emails.Impl;
using Cluster.System.Impl;
using Cluster.System.Mock;
using Cluster.Accounts.Impl;
using Cluster.Audit.Impl;
using Cluster.Accounts.Test;
using Cluster.Forms.Test;
using Cluster.Batch.Impl;
using NakedObjects.Async;
using App.DataAccess;
using Cluster.Users.Impl;

namespace App.BatchRun {
     public class RunExe : RunBatch {

         protected override IServicesInstaller MenuServices
         {
             get
             {
                 return new ServicesInstaller(
                     new AccountsService(),
                     new UserService(),
                     new AuditService(),
                     new TaskRepository(),
                     new DocumentService(),
                     new EmailService(),
                     new BatchRepository(),
                     new CustomerAccountsService(),
                     new FormRepository(),
                     new FormService(),
                     new MockFormInitiator()
                     );
             }
         }

         protected override IServicesInstaller ContributedActions
         {
             get
             {
                 return new ServicesInstaller(new TaskContributedActions(), new AuditContributedActions());
             }
         }

         protected override IServicesInstaller SystemServices
         {
             get
             {
                 return new ServicesInstaller(
                     new AdjustableClock(),
                     new PolymorphicNavigator(),
                     new NameService(),
                     new NakedObjectsEmailSender(),
                     new XmlSnapshotService(),
                     new SimpleCustomerAccountNumberCreator(),
                     new FormService(), 
                     new BatchProcessRunner(),
                     new AsyncService()
                     );
             }
         }

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                var installer = new EntityPersistorInstaller();
                //Database.SetInitializer(new AppInitializer());
                installer.UsingCodeFirstContext(() => new AppDbContext());
                return installer;
            }
        }

         public static void Run() {
            new RunExe().Start(new StartPoint());
        }

         protected override void InitialiseLogging()
         {
            // throw new System.NotImplementedException();
         }
     }
}