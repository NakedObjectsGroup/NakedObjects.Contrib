using NakedObjects.Boot;
using NakedObjects.Core.Context;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;

namespace Cluster.Batch.Test {
     public class RunExe : RunBatch {
        protected override NakedObjectsContext Context {
            get { return HttpContextContext.CreateInstance(); }
        }

        protected override IServicesInstaller MenuServices {
            get {
                return new ServicesInstaller();
            }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(); }
        }

        //protected override IServicesInstaller SystemServices {
        //    get { return new ServicesInstaller(new SimpleEncryptDecrypt()); }
        // }


		// example functions that gets types for AssociateTypes below  
		//private static Type[] AdventureWorksTypes() {
        //    var allTypes =  AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
        //    return allTypes.Where(t => t.BaseType == typeof(AWDomainObject) && !t.IsAbstract).ToArray();
        //}
		//
		//private static Type[] CodeFirstTypes() {
        //    return new[] {typeof(Class1), typeof(Class2)};
        //}

        protected override IObjectPersistorInstaller Persistor
        {
            get
            {
                // Database.DefaultConnectionFactory = new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0"); //For in-memory database
                // Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyDbContext>()); //Optional behaviour for CodeFirst
                var installer = new EntityPersistorInstaller();

                // installer.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes); // for Model/Database First
                // installer.UsingCodeFirstContext(() => new MyDbContext()).AssociateTypes(CodeFirstTypes);  //For Code First

                return installer;
            }
        }


       public static void Run(string[] toRun) {
            new RunExe().Start(null);
        }

       protected override void InitialiseLogging()
       {
           //throw new global::System.NotImplementedException();
       }
     }
}