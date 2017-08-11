namespace Cluster.Templates.Api
{
    /// <summary>
    /// Implemented by any domain object that, directly or indirectly, can provide
    /// fields to be merged into a document
    /// </summary>
    public interface IMergeFieldsProvider
    {
        /// <summary>
        /// Throws exception if fieldName not recognised
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetContentsFor(string fieldName);
    }
}
