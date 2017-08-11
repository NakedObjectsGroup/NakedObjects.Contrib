namespace Cluster.Emails.Api
{
    /// <summary>
    /// Role interface implemented by external object that has EmailDetails managed by the Emails cluster.
    /// </summary>
    public interface IHasEmailDetails : IEmailAddressProvider
    {
        //Persistent property holding the FK for the EmailDetails object.
        int EmailDetailsId { get; }
    }
}
