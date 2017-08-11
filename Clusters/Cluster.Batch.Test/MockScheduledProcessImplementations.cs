using System;
using System.Collections.Generic;
using System.Linq;
using Cluster.Batch.Api;
using System.Threading;

namespace Cluster.Batch.Test
{
    public class MockSP : IServiceBatchProcess
    {
        public string Invoke()
        {
            return "MockSP run OK";
        }

        public string[] DeriveKeys()
        {
            throw new NotImplementedException();
        }

        public void PopulateUsingKeys(string[] keys)
        {
            throw new NotImplementedException();
        }
    }

    public class MockSPThrowsException : IServiceBatchProcess
    {
        public string Invoke()
        {
            throw new Exception("Test Exception");
        }

        public string[] DeriveKeys()
        {
            throw new NotImplementedException();
        }

        public void PopulateUsingKeys(string[] keys)
        {
            throw new NotImplementedException();
        }
    }

    public class MockPersistentSP : IPersistentBatchProcess
    {

        public string Invoke()
        {
            return SpecificOutcome;
        }
       
        public virtual int Id { get; set; }
       
        public virtual string SpecificOutcome { get; set; }
      
      
    }

    public class MockNotAnSP { }

    public class SleepingBatchProcess : IPersistentBatchProcess
    {


        public string Invoke()
        {
            Thread.Sleep(SleepForMilliseconds);
            return "Slept for " + SleepForMilliseconds + " ms";
        }

        
        public virtual int Id { get; set; }

        
        public virtual int SleepForMilliseconds { get; set; }
      

    }
}
