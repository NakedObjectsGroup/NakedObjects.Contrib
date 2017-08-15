// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Persistor.Entity.Configuration;
using Template.DataBase;
using Template.SeedData;
using Cluster.Tasks.Impl;
using Cluster.Forms.Impl;
using Cluster.Users;
using Cluster.Addresses.Impl;
using Cluster.Tasks.Test;
using Cluster.Names.Impl;
using Cluster.Addresses.Test;
using Cluster.Documents.Impl;
using Cluster.Documents.Test;
using Cluster.Users.Impl;
using Cluster.Emails.Test;
using Cluster.Emails.Impl;
using Cluster.System.Impl;
using Cluster.System.Mock;
using Cluster.Accounts.Impl;
using Cluster.Audit.Impl;
using NakedObjects.Snapshot;
//using Cluster.Users.Test;
using Cluster.Accounts.Test;
using Cluster.Countries.Impl;
using Cluster.Forms.Test;
using Cluster.Telephones.Impl;
//using Cluster.Batch.Impl;
using NakedObjects.Services;
using NakedObjects.Snapshot.Xml.Service;
using NakedObjects.Meta.Authorization;
using NakedObjects.Meta.Audit;

namespace NakedObjects.Template {
    public class NakedObjectsRunSettings
    {

        public static string RestRoot
        {
            get { return ""; }
        }

        private static string[] ModelNamespaces
        {
            get
            {
                return new string[] { "Cluster" }; //Add top-level namespace(s) that cover the domain model
            }
        }

        private static Type[] Types
        {
            get
            {
                return new Type[] {
                    //You need only register here any domain model types that cannot be
                    //'discovered' by the framework when it 'walks the graph' from the methods
                    //defined on services registered below
                };
            }
        }

        private static Type[] Services
        {
            get
            {
                return new Type[] {
                     typeof(AccountsService),
                    typeof(UserService),
                    typeof(AuditService),
                    typeof(TaskRepository),
                    typeof(DocumentService),
                    typeof(EmailService),
                    //typeof(BatchRepository),
                    //typeof(BatchProcessRunner),
                    typeof(TaskContributedActions),
                    typeof(AuditContributedActions),
                    typeof(AdjustableClock),
                    typeof(PolymorphicNavigator),
                    typeof(NameService),
                    typeof(NakedObjectsEmailSender),
                    typeof(XmlSnapshotService),
                    typeof(SimpleCustomerAccountNumberCreator),
					typeof(FormService),
					typeof(CountryService),
					typeof(TelephoneService),
					typeof(AddressService),
                    //typeof(ProcessDefinitionRepository),
                    //typeof(CustomerAccountsService),
                    //typeof(FormRepository),
                    //typeof(FormService),
                    //typeof(MockFormInitiator)
                };
            }
        }

        public static ReflectorConfiguration ReflectorConfig()
        {
            return new ReflectorConfiguration(Types, Services, ModelNamespaces, MainMenus);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig()
        {
            var config = new EntityObjectStoreConfiguration();
            config.UsingCodeFirstContext(() => new AppDbContext("NakedObjectsTemplate", new AppInitializer()));
            return config;
        }

        public static IMenu[] MainMenus(IMenuFactory factory)
        {
            return new IMenu[] {
               factory.NewMenu<AccountsService>(true, "Accounts"),
               factory.NewMenu<UserService>(true, "Users"),
			   factory.NewMenu<AddressService>(true, "Addresses"),
			   factory.NewMenu<CountryService>(true), // TODO: disable later
               factory.NewMenu<AuditService>(true, "Audit"),
               factory.NewMenu<DocumentService>(true, "Documents"),
               factory.NewMenu<EmailService>(true, "Email")
            };
        }

        //public static IAuthorizationConfiguration AuthorizationConfig()
        //{
        //    var config = new AuthorizationConfiguration<DefaultAuthorizerForSysAdminOnly>();
        //    config.AddNamespaceAuthorizer<AuditAuthorizer>("Cluster.Audit.Impl");
        //    config.AddNamespaceAuthorizer<TasksAuthorizer>("Cluster.Tasks.Impl");
        //    config.AddNamespaceAuthorizer<UsersAuthorizer>("Cluster.Users.Impl");
        //    return config;
        //}

        private static IAuditConfiguration MyAuditConfig()
        {
            var config = new AuditConfiguration<DefaultAuditor>();
            return config;
        }


    }
}