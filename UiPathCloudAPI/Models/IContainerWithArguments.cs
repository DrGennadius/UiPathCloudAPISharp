namespace UiPathCloudAPISharp.Models
{
    /// <summary>
    /// Interface for containers that contain arguments.
    /// </summary>
    public interface IContainerWithArguments
    {
        /// <summary>
        /// Get input and output arguments as dictionaries.
        /// </summary>
        /// <returns></returns>
        Arguments GetArguments();
    }
}