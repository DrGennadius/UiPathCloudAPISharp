namespace UiPathCloudAPISharp.Query
{
    public interface IQueryStringTransform
    {
        /// <summary>
        /// Get OData String using input String
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string GetQueryString(string input);
    }
}