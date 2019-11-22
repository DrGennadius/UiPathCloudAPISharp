namespace UiPathCloudAPISharp
{
    public interface IODataTransform
    {
        /// <summary>
        /// Get OData String using input String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string GetODataString(string input);
    }
}